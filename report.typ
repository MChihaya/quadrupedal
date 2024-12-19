#import "libs/rsj-conf/lib.typ": rsj-conf
#show: rsj-conf.with(
  title: [AIALレポート], 
  authors: [森田　知颯（03240454）],
  bibliography: bibliography("refs.yml", full: false)
)

#import "@preview/sourcerer:0.2.1": code
#show link: underline
#import "@preview/codelst:2.0.1": sourcecode
= 概要
本レポートは演習課題Cの四足歩行ロボットの学習に関するレポートである。

一定時間ごとのロボットの足のジョイントの角度を定めた実数値の羅列を遺伝子とする場合とニューラルネットワークを用いてその重み行列の要素を遺伝子とする場合、その両方を掛け合わせた場合で、ロボットに歩行を学習させることを目的として課題に取り組んだ

= 足の角度を直接遺伝子とする場合
== 目標なし
=== 方法
=== 結果
=== 考察
== 目標あり
=== 方法
=== 結果
=== 考察

= ニューラルネットワークの重み行列を遺伝子とする場合
== 足の角度を直接出力とする場合
=== 方法
=== 結果
=== 考察

== 2で学習した足の動きを再生させる場合
=== 方法
=== 結果
=== 考察
