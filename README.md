# ClassifiedConsoleWindow
Unity Classified Console Window

一款 [可分类] [带控制台] [可局域网连接调试] [可查询历史] 的Unity日志系统

性能比较于Unity原生的Console系统，GC更少，速度更快

*在不带有栈信息，输入的Msg信息不带字符串拼接的情况下，可以做到了0GC

### 安装

https://openupm.cn/packages/com.gm.classifiedconsolewindow/

### 使用方式

``` C#
using ClassifiedConsole;
CDebug.Log();
CDebug.LogWarning();
CDebug.LogError();
// 其中 参数 [int skipLine] 为 需要额外跳过的栈行数
// 其中 参数 [int subSystem]  为 该条 Log 的对应分类
```

*注：为什么没有 CDebug.LogException()？，因为，日志系统和异常管理系统，是两个系统，专事专干

但这并不代表ClassifiedConsoleWindow看不了异常的信息，所有抛出未处理的异常也都会被捕获并显示，归为Unity_Native_Log分类

### 打开配套的Editor 窗口

菜单 Window/Classified/Console 打开 Console 窗口

窗口基于 UnityEngine.UIElements 编写

![CDebugWindow](https://github.com/Goatman1996/ClassifiedConsoleWindow/assets/48623605/3e16edcc-80f2-4d07-b2f4-fc35f38c38eb)
Archive按钮，可将当前日志归档

Current_xxx，为选择历史记录菜单

Editor下拉菜单，可连接真机，输入真机端的Ip地址即可

左侧为日志的分类菜单，可选择性的开关显示

在右下角的Log详情窗口中，右击，会有[Hide Async Method Stack]选项，勾选，可以优化async/await的栈信息

### 打开相关的设置窗口

菜单 Window/Classified/Settings 打开Console Settings 窗口

Settings窗口内的所有设置项均包含说明，详情可在面板查看

![CDebugSettings](https://github.com/Goatman1996/ClassifiedConsoleWindow/assets/48623605/85acf45a-6eb1-48fd-9089-dd72d6f7dfed)

额外说明：

*Catch Native XXX （是否监听运行时XXX）选项

即本系统是否需要捕获 UnityEngine.Debug.LogXXX() 的信息，并归为Unity_Native_Log分类

仅运行时捕获

*Sub System Defined Assembly 

内容是所有使用了[CDebugSubSystemAttribute]标签的程序集

*以下是Settings相关的API

``` C#
// Console Settings 窗口 包含了 大部分能配置的内容
// 出包时，可为包进行特别设定
// 以下是代码层面的相关API
void Settins()
{
    // 由此获得并修改，Console Settings中的内容
    var settings = ClassifiedConsole.CDebugSettings.Instance;

    ...
    // 修改 Settings中的内容
    ...

    // 保存
    ClassifiedConsole.CDebugSettings.Save();
}
```
### 日志分类
分类标签 [ClassifiedConsole.CDebugSubSystem] 为 枚举类型 的属性，为 枚举类型添加此标签，即可在 配套窗口中 中显示此分类，

分类标签 起名 [ClassifiedConsole.CDebugSubSystemLabel] 为 特定分类类型的别称，用于在窗口中显示

``` C#
using ClassifiedConsole;
// 这个标签是 用于标记 特定的枚举类型 为 Log分类类型
[CDebugSubSystem]
public enum LoggerType
{
    [CDebugSubSystemLabel("启动器")]
    Launcher = 0,
    UI = 1,
    ...
}
```
### 局域网 调试真机（基于Http）
Console Window 包含了连接真机的功能

``` C#
void StartRemoteListener()
{
    // 真机上调用，一次开启端口监听，端口号可在Settings中配置
    // 在 Console Window 中 填入 真机局域网IP,即可连接真机看Log
    // (如果不填写端口号，系统会自动获取Settings中配置的端口号)
    string url = ClassifiedConsole.CDebugRemoteListener.Start(true);

    // 参数bool ignoreEditor为，是否忽略Editor（Editor下不监听端口），默认为true
    // 如果Editor也监听，Remote为本机环境，会导致端口监听冲突
}
```

### 控制台工具

![CDebugCMD](https://github.com/Goatman1996/ClassifiedConsoleWindow/assets/48623605/cbb4ac86-baaf-4b1c-8474-4a14a5bd390f)

在Console窗口的最下方有一个输入框，输入完整的（静态）方法名，可调起静态方法。

参数使用空格分开（支持大部分默认的值类型）

完整名称例子:[namespace].[class].[method] [param1] [param2] [...]

上下方向键，可查看成功运行的命令历史

Enter键为，执行命令，运行目标函数

Editor环境下，[Run]按钮绿色时，即为查询到命令相应的方法

真机调试 时，[Run]按钮不会反应真实情况，红色也有可能可以运行

*注：目前只支持调用在Setttings.SubSystemDefinedAssembly中，第一个程序集里的静态方法

### 一些小细节

在使用git作为版本管理工具时，可将ClassifiedConsole添加到.gitignore中

这个文件夹是Editor下日志存放的位置

### 最后

觉得有趣的点个Star~

谢谢~
