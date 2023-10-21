# FindDup4Disk：多机器查找重复文件
FindDup4Disk可以帮你扫描多台机器上的文件，根据其MD5值判断是否重复。

## 运行实例

## 安装
需要先安装Windows .net6。
下载打包好的release包解压就可以直接运行。

## 一些小提示
1. 本软件支持多台机器上的文件扫描和文件重复比较；

2. 扫描完本机后，去扫描新的机器时，请确保将本机C:\daba.db拷贝到新机器的同样位置C:\daba.db；

3. 新机器上同样安装本软件；扫描新机器的磁盘；

4. 在新机器上进行文件比较；

5. 若想在本机上做跨机器的文件比较，请先确保将新机器上最新的C:\daba.db文件拷贝回本机；

6. 多台机器比较时，请总确保所操作机器上的C:\daba.db文件是最新的。

## 常见问题

## 代码结构

## 代码规范

1：类名使用 Pascal 大小写形式
public class HelloWorld{ ...}

2：方法使用 Pascal 大小写形式
public class HelloWorld{ void SayHello(string name) {  ... }}

3：变量和方法参数使用 驼峰式 大小写形式
Public int totalCount = 0;

4：根据类的具体情况进行合理的命名
以Class声明的类，都必须以名词或名词短语命名，体现类的作用。如：

Class Indicator {}

当类只需有一个对象实例（全局对象，比如Application等），必须以Class结尾，如

Class ScreenClass

当类只用于作为其他类的基类，根据情况，以Base结尾：

Class IndicatorBase

5：不要使用匈牙利方法来命名变量 （m_xxxx）
string m_sName; int nAge;

6：用有意义的，描述性的词语来命名变量
别用缩写。用name, address, salary等代替 nam, addr, sal

别使用单个字母的变量象i, n, x 等. 使用 index, temp等

用于循环迭代的变量例外：

string m_sName; int nAge;

7：文件名要和类名匹配
例如，对于类HelloWorld, 相应的文件名应为 HelloWorld.cs

8：接口的名称前加上I
interface ImyInterface   {……}

9：在私有成员变量前面加上m_。对于m_后面的变量名使用骆驼命名方法：（注意第五条）
public class SomeClass  
{  
    private int m_Number;   
}
https://blog.csdn.net/qq_39708228/article/details/131652620


