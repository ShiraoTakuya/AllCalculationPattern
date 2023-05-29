# 全ての部品の全ての公差の組み合わせを計算

全ての公差計算のパターンを行い、全ての結果とTYP,MAX,MIN結果を出力します。
 
# 実行例

RLC直列回路のインピーダンスの大きさ計算

パラメータ設定
* Math.Sqrt(Math.Pow(R1,2)+Math.Pow(ω*L1-1/(ω*C1),2))
* R1,10e3,0.05
* ω,6e3,0.1
* L1,1e-6,0.1
* C1,10e-6,0.1

実行結果

![demo](https://raw.githubusercontent.com/ShiraoTakuya/Csharp_Repositories/main/AllCalculationPattern/cap.PNG)
 
# Requirement
 
C#のコンパイラが必要です。
※Windowsであればデフォルトで入っています
 
# Usage

以下の手順を行う
* source.exeを実行
* Windowの1行目に計算式を入力する
* Windowの2行目以降に記号の定義を入力。"記号,定数,公差"の順番。
* F5キーを押す
 
# Note
 
* コンパイラコンパイラとして動作を実現しています。
 
# Author
  
* ShiraoTakuya
