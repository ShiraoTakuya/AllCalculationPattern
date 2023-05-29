using System;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Collections.Generic;

//メイン
class program{

	[STAThreadAttribute]
	static void Main(){
		var iForm = new Window();
		Application.Run(iForm);
	}
}

//ウィンドウクラス
class Window: Form{

	[DllImport("user32.dll")]
	public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr NewParent);

	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(String stName, String stClass);

	//変数宣言
	public List<TextBoxEx> arTextBox;

	//コンストラクタ
	public Window(){
		//ウィンドウ初期化
		WindowInitialize();

		//テキストボックス初期化
		TextboxInitialize();

		//デフォルトの計算をテキストボックスに追加
		using(var sr = new StreamReader("DefaultCalculation", Encoding.GetEncoding("Shift_JIS"))){
			arTextBox[0].Text = sr.ReadToEnd();
		}
	}

	//ウィンドウ初期化処理
	public const int WINDOW_SIZE_X = 640;
	public const int WINDOW_SIZE_Y = 480;
	private void WindowInitialize(){
		Icon = new Icon("favicon.ico");
		ClientSize = new Size(WINDOW_SIZE_X, WINDOW_SIZE_Y);
		Text = "All Calculation Pattern";
		FormBorderStyle = FormBorderStyle.FixedSingle;
	}

	//テキストボックス初期化処理
	public void TextboxInitialize(){
		int[,] SIZE_INFO = new int[,]{
			{0, 0, 640, 480}
		};
		//テキストボックスを生成
		arTextBox = new List<TextBoxEx>();
		for(var i = 0; i < SIZE_INFO.GetLength(0); i++){
			arTextBox.Add(new TextBoxEx());
		}

		//テキストボックスをウィンドウに追加
		for(int i = 0; i < arTextBox.Count; i++){
			var tb = arTextBox[i];
			tb.Font = new Font("ＭＳ Ｐゴシック", 9);
			tb.Size = new Size(SIZE_INFO[i,2], SIZE_INFO[i,3]);
			tb.Location = new Point(SIZE_INFO[i,0], SIZE_INFO[i,1]);
			tb.Text = "";
			tb.KeyDown += new KeyEventHandler(Form1_KeyDown);
			tb.Multiline = true;
			tb.BorderStyle = 0;
//			tb.MouseDown += new MouseEventHandler(Form1_MouseDown);
//			tb.MouseMove += new MouseEventHandler(Form1_MouseMove);
			tb.WatermarkText = "Press F5 for making a All Calculation Results";
			Controls.Add(tb);
		}

		
	}

	//KeyDown動作
	void Form1_KeyDown(object sender, KeyEventArgs e){
		if(e.KeyCode == Keys.F5){
			//ここにしたい処理を追加
			ExtRegex();
		}
		if(e.KeyCode == Keys.F4){
			if((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
				Application.Exit();
		}
	}

	//正規表現抽出
	private void ExtRegex(){
		var stTemplate = "";
		using(var sr = new StreamReader("template", Encoding.GetEncoding("Shift_JIS"))){
			stTemplate = sr.ReadToEnd();
		}
		using(var sw = new StreamWriter("CompiledSource.cs")){
			//テキストボックスに入力した文字列を取得
			var stTextBoxStrings = arTextBox[0].Text;

			//stTextBoxStringsの内容を下位行区切りでarTextBoxStringsに格納
			var stPattern = "(\r\n)+";
			var stReplaceString = "\r\n";
			stTextBoxStrings = Regex.Replace(stTextBoxStrings, stPattern, stReplaceString, RegexOptions.Singleline);
			stTextBoxStrings = Regex.Replace(stTextBoxStrings, "(^\r\n|\r\n$)", "", RegexOptions.Singleline);
			var arTextBoxStrings = Regex.Split(stTextBoxStrings, "\r\n");

			//テキストボックスから情報を配列に抽出
			var stCal = arTextBoxStrings[0];
			var arParam = new List<string[]>();
			for(int i = 1; i < arTextBoxStrings.Length; i++){
				arParam.Add(arTextBoxStrings[i].Split(','));
			}

			//数式をarCal[i]の形に修正
			for(int i = arParam.Count - 1; i >= 0; i--){
				stCal = stCal.Replace(arParam[i][0], String.Format("arCal[{0}]", i));
			}

			//定数と公差を配列チックに格納
			var stVar = arParam[0][1];
			var stTol = arParam[0][2];
			for(int i = 1; i < arParam.Count; i++){
				stVar += "," + arParam[i][1];
				stTol += "," + arParam[i][2];
			}

			//テンプレートを修正
			stTemplate = stTemplate.Replace("<変数宣言>", stVar).Replace("<公差宣言>", stTol).Replace("<計算式>", stCal);
			sw.Write(stTemplate);

			//外部コマンド実行(コンパイルと実行)
			Process.Start(@"CompleComple.vbs");
		}
	}

	//テキストボックスドラッグ
	private Point mousePoint;
	private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e){
		if ((e.Button & MouseButtons.Left) == MouseButtons.Left){
			mousePoint = new Point(e.X, e.Y);
		}
	}
	private void Form1_MouseMove(object sender,	System.Windows.Forms.MouseEventArgs e){
		if ((e.Button & MouseButtons.Left) == MouseButtons.Left){
			foreach(TextBoxEx tb in arTextBox){
				int iX = tb.Location.X + e.X - mousePoint.X;
				int iY = tb.Location.Y + e.Y - mousePoint.Y;
				tb.Location = new Point(iX, iY);
			}
		}
	}

}

//テキストボックスにウォーターマークを追加
class TextBoxEx: TextBox{
	public String WatermarkText = "";
	protected override void WndProc(ref Message m){
		const int WM_PAINT = 0x000F;
		base.WndProc(ref m);
		if (m.Msg == WM_PAINT && string.IsNullOrEmpty(this.Text) && string.IsNullOrEmpty(WatermarkText) == false){
		using (Graphics g = Graphics.FromHwnd(this.Handle)){
				Rectangle rect = this.ClientRectangle;
				rect.Offset(1, 1);
				TextRenderer.DrawText(g, WatermarkText, this.Font, rect, SystemColors.ControlDark, TextFormatFlags.Top | TextFormatFlags.Left);
			}
		}
	}
}