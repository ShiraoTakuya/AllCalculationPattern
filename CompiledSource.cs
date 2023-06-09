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

		//計算を実行
		ExtRegex();
	}

	//ウィンドウ初期化処理
	public const int WINDOW_SIZE_X = 320;
	public const int WINDOW_SIZE_Y = 240;
	private void WindowInitialize(){
		Icon = new Icon("favicon.ico");
		ClientSize = new Size(WINDOW_SIZE_X, WINDOW_SIZE_Y);
		Text = "Results";
		FormBorderStyle = FormBorderStyle.FixedSingle;
	}

	//テキストボックス初期化処理
	public void TextboxInitialize(){
		int[,] SIZE_INFO = new int[,]{
			{0, 0, 320, 240}
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
			tb.WatermarkText = "";
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
		//arVarに値、arTolに公差を格納
		var arVar = new List<double>(){10e3,6e3,1e-6,10e-6};
		var arTol = new List<double>(){0.05,0.1,0.1,0.1};
		var arIndex = new List<BitArray>();

		//2^要素数回分のパターンを格納
		for(int i = 0; i < Math.Pow(2, arVar.Count); i++){
			arIndex.Add(new BitArray(new int[] {i}));
		}

		//2^要素数回分の計算処理
		var arResults = new List<double>();
		var arCal = new List<double>();
		for(int i = 0; i < Math.Pow(2, arVar.Count); i++){
			//arCalに1回分の計算値を格納
			arCal = new List<double>();
			for(int j = 0; j < arVar.Count; j++){
				int iIndexSign = Convert.ToInt32(arIndex[i][j]);
				if(iIndexSign == 0) iIndexSign = -1;
				arCal.Add(arVar[j]*(1 + arTol[j] * iIndexSign));
			}
			//計算結果を出力
			arResults.Add(Math.Sqrt(Math.Pow(arCal[0],2)+Math.Pow(arCal[1]*arCal[2]-1/(arCal[1]*arCal[3]),2)));
		}

		//TYP値を計算出力
		arCal = new List<double>();
		for(int j = 0; j < arVar.Count; j++){
			arCal.Add(arVar[j]);
		}
		arTextBox[0].Text = "TYP値: "+Math.Sqrt(Math.Pow(arCal[0],2)+Math.Pow(arCal[1]*arCal[2]-1/(arCal[1]*arCal[3]),2))+"\r\n";

		//MAX,MINを出力
		var dMax = arResults[0];
		var dMin = arResults[0];
		foreach(double dVal in arResults){
			dMax = Math.Max(dVal, dMax);
			dMin = Math.Min(dVal, dMin);
		}
		arTextBox[0].Text += "MAX値: "+dMax+"\r\n";
		arTextBox[0].Text += "MIN値: "+dMin+"\r\n";

		//結果を出力
		arTextBox[0].Text += string.Join("\r\n",arResults);
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