# SummerComboSystem
ComboSystem
结合xNode可视化编辑的Combo系统。

开始编写Combo实际效果的函数代码

自定义ComboConfig类，类中的成员为配表中会配置的Combo数据。
自定义ComboConfig类中须包含的成员。

 int Id //ComboId
 
 float Duration //Combo持续时间
 
 string TimeLine //时间轴
 
该ComboSystem将一个Combo释放过程中会调用的函数拆分为由TimeLine中的TimeLinePoint调用的一个个单个函数。
TimeLine使用“|”将TimeLinePoint的调用名称和调用时间分隔开，如"Init|0|Calc|0.5"即为0秒时调用该Combo的相应Init函数，0.5秒时调用该Combo的相应Calc函数。

MainId And SubId

单个Combo的ComboId是由其TimeLine中所有TimeLinePoint调用函数的SubId+它们所在类的MainId构成的，在使用Combo时会以这样的原则找到对应的MethodInfo。

ComboModel And ComboProcessor

创建一个带有ComboModel(MainId)特性的类用于写入一套Combo(推荐用一套Model对应一个游戏character的所有Combo)中所有时间点会调用的静态函数。
这些静态函数须带有ComboProcessor(funcName,SubId)特性。其中funcName参数为在配表的TimeLine中表示一个TimeLinePoint的名称，如Init、Calc。
注：静态方法的参数，同一概念请使用同一参数名，如Combo命令的发送者参数统一为sender。

Combo数据配表的创建

一套ComboModel使用一份配表，配表使用csv文件，文件路径为Assets/AssetsPackage/Datas，配表命名为该ComboModel的类名，配表的第三行须为对应ComboConfig的各成员名，之后各行写入Combo的对应参数。其中TimeLine列的参数形式应为"funcName1|调用时间|funcName2|调用时间"。

配置Combo的派生顺序及连Combo的使用输入

create->NodeGraph->ComboGraph创建配置图。
在配置图中先创建StartCombo节点作为连招的初始Combo，之后创建NormalCombo节点将节点之间相连编辑派生顺序。
节点中通过填入ComboId决定该节点的对应Combo，配置MouseInput和KeyBoardInput决定该combo的使用输入。

使用ComboSystem

将GM-ComboMgr挂载到一个游戏物体上。
在角色的游戏物体上挂载ComboControl，将配置的ComboGraph拖入Inspector的ComboGraph中。
通过调用ComboControl中的ChangeuData(string 方法参数名,object 参数内容)来为Combo使用时调用的方法提供参数值。
按照编辑的Combo输入即可使用Combo。
