# ClassifiedConsoleWindow
Unity Classified Console Window

一款 [可分类] [带控制台] [可局域网连接调试] [可查询历史] Unity日志系统

性能比较于Unity原生的Console系统，GC更少，速度更快

*在不带有栈信息，输入的Msg信息不带字符串拼接的情况下，可以做到了0GC

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
![CDebugWindow](https://github.com/Goatman1996/ClassifiedConsoleWindow/assets/48623605/3e16edcc-80f2-4d07-b2f4-fc35f38c38eb)
Archive按钮，可将当前日志归档

Current_xxx，为选择历史记录菜单

Editor下拉菜单，可连接真机，输入真机端的Ip地址即可

左侧为日志的分类菜单，可选择性的开关显示
### 打开相关的设置窗口

菜单 Window/Classified/Settings 打开Console Settings 窗口

Settings窗口内的所有设置项均包含说明，详情可打开面板查看

``` C#
// Console Settings 窗口 包含了 大部分能配置的内容
// 以下是代码层面的相关API
void Settins()
{
    // 由此获得并修改，Console Settings中的内容
    var settings = ClassifiedConsole.CDebugSettings.Instance;
    {
        // 修改 Settings中的内容
    }
    // 保存
    ClassifiedConsole.CDebugSettings.Save();
}
```
### 日志分类相关
分类标签 [ClassifiedConsole.CDebugSubSystem] 为 枚举类型 的属性，为 枚举类型添加此标签，即可在 配套窗口中 中显示此分类，

分类标签 起名 [ClassifiedConsole.CDebugSubSystemLabel] 为 特定分类类型的别称，用于在窗口中显示

``` C#
// 这个标签是 用于标记 特定的枚举类型 为 Log分类类型
[ClassifiedConsole.CDebugSubSystem]
private enum TestType
{
    [ClassifiedConsole.CDebugSubSystemLabel("这是第一个标签")]
    MyLog = 0,
    MyLog1 = 1,
}
```
### 局域网 调试真机（基于Http）
Console Window 包含了连接真机的功能

``` C#
void StartRemoteListener()
{
    // 真机上调用，一次开启端口监听，端口号可在Settings中配置
    // 在 Console Window 中 填入 真机局域网IP,即可连接真机看Log
    string url = ClassifiedConsole.CDebugRemoteListener.Start.Start(false);
}
```

### 命令行工具

在Console窗口的最下方有一个输入框，输入完整的（静态）方法名，可调起静态方法。

参数使用空格分开（支持大部分默认的值类型）

完整名称例子:[namespace].[class].[method] [param1] [param2] [...]

局域网 真机调试 亦可
