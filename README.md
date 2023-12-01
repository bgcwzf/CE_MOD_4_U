Hi，请把当前仓库，clone到【C:\】下，因为在【CT\DSPGAME.CT】文件里面有一些对文件的访问以硬编码的方式，写在文件里面了。


当前项目的大致使用流程如下：


1.把当前文件夹中的子文件夹“autorun”中的所有文件，复制到Cheat Engine 7.4+主程序所在的文件夹中的“autorun”子文件夹中。


2.编写MOD代码。
  主要通过建立一个基于.net framework 4.0的C#项目。使用VS2019或更新版本，来打开“DysonSphereProgram_MODs\DSP_MODs.sln”来查看示例


3.启动游戏。并用CE打开游戏进程


4.再用CE载入这里的、与游戏名称对应的CT文件


5.在CE中，添加、修改里面的MOD条目


6.激活MOD & Enjoy ！ 