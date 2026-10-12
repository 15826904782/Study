# C# / .NET 面试题库（2026 版）

> 本文档基于全网公开资料检索整理，覆盖 **C# 语言基础、.NET 运行时、异步与并发、ASP.NET Core、Entity Framework Core、数据库与缓存、设计模式、微服务架构、性能优化、测试** 等核心方向，难度从初级到高级，共 13 个章节、260+ 道题。
>
> 使用建议：
> - **初级候选人**：重点复习第 1、4、5 章基础题；
> - **中级候选人**：重点复习第 2、3、5、6、7 章；
> - **高级候选人**：重点复习第 3、6、7、8、9、10、11 章及第 12 章场景题；
> - 每道题答案给出**核心要点**而非长篇大论，适合背诵与自查，关键概念附代码示例。
>
> 参考来源见文末附录。

---

## 目录

1. [C# 语言基础](#c1)
2. [C# 进阶与 .NET 运行时](#c2)
3. [异步编程与多线程并发](#c3)
4. [.NET 平台与 C# 新特性](#c4)
5. [ASP.NET Core](#c5)
6. [Entity Framework Core](#c6)
7. [数据库与缓存](#c7)
8. [设计模式与架构原则](#c8)
9. [微服务与分布式系统](#c9)
10. [性能优化与内存管理](#c10)
11. [测试与质量保障](#c11)
12. [综合场景与行为面试](#c12)
13. [附录：高频速记表与推荐资料](#c13)

---

<a id="c1"></a>

## 一、C# 语言基础

### 1.1 值类型与引用类型的区别？（必问）

**要点：**
- **存储位置**：值类型直接存数据，通常分配在栈上（或作为引用类型的字段内联）；引用类型变量存的是堆上对象的地址，对象本体在托管堆。
- **赋值行为**：值类型赋值是复制副本，互不影响；引用类型赋值只复制引用，多个变量指向同一对象。
- **默认值**：值类型默认值为 0 / false；引用类型默认值为 `null`。
- **包含类型**：值类型包括 `int`、`double`、`bool`、`char`、`enum`、`struct`、可空类型；引用类型包括 `class`、`interface`、`delegate`、`array`、`string`。
- **继承**：值类型隐式继承 `System.ValueType`（不能继承其他类）；引用类型可继承其他类。

**加分点**：`string` 虽然是引用类型，但具有**不可变性**，表现为值语义；`record class` 也是引用类型但提供值相等性。


**代码示例：**

```csharp
// 值类型：赋值是复制副本，互不影响
int a = 10;
int b = a;
b = 20;
Console.WriteLine(a); // 10，a 不受影响

// 引用类型：赋值只复制引用，多个变量指向同一对象
var p1 = new Person { Name = "张三" };
var p2 = p1;
p2.Name = "李四";
Console.WriteLine(p1.Name); // 李四，p1/p2 指向同一个对象

class Person { public string Name { get; set; } }
```

### 1.2 什么是装箱（Boxing）和拆箱（Unboxing）？如何避免？

**要点：**
- **装箱**：值类型 → `object` / 接口，在堆上分配对象并复制值，有性能开销。
- **拆箱**：`object` / 接口 → 值类型，先检查类型匹配（否则抛 `InvalidCastException`），再复制值。
- **性能损耗**：装箱产生堆分配和拷贝，频繁装箱会加重 GC 压力。
- **避免方式**：
  - 使用**泛型**（`List<int>` 而非 `ArrayList`）；
  - 避免把值类型当 `object`/接口传参；
  - 重写 `ToString()` 时用 `$"值：{x}"`（现代编译器对常见场景已优化）或 `x.ToString()` 而非拼接 object；
  - 用 `Nullable<T>` / `int?` 替代装箱的空值判断。


**代码示例：**

```csharp
int x = 42;
object obj = x;      // 装箱：值类型 -> object，堆上分配对象
int y = (int)obj;    // 拆箱：先检查类型匹配，再复制值（类型不匹配抛 InvalidCastException）

// 避免装箱：用泛型集合，而不是 ArrayList
var list = new List<int> { 1, 2, 3 };   // 无装箱
var list2 = new ArrayList { 1, 2, 3 };  // 每个元素都装箱，性能差
```

### 1.3 `string` 与 `StringBuilder` 的区别？

- `string` **不可变**，任何修改都会创建新字符串对象；大量拼接会产生大量垃圾对象。
- `StringBuilder` 是可变字符缓冲区，适合**循环内大量拼接**场景。
- 少量拼接（如 3~5 次）编译器/运行时已优化（字符串字面量拼接编译期完成），用 `string` 即可。
- 建议：循环拼接用 `StringBuilder`；一次性拼接可用 `$` 插值（内部走 `DefaultInterpolatedStringHandler`，已优化）。


**代码示例：**

```csharp
// 错误：循环内用 string 拼接，每次产生新字符串，产生大量垃圾对象
string s = "";
for (int i = 0; i < 10000; i++) s += i;

// 正确：用 StringBuilder，内部是可变字符缓冲区
var sb = new StringBuilder();
for (int i = 0; i < 10000; i++) sb.Append(i);
string result = sb.ToString();
```

### 1.4 `ref`、`out`、`in` 参数的区别？

| 关键字 | 传参前是否必须初始化 | 方法内是否必须赋值 | 用途 |
|---|---|---|---|
| `ref` | 必须 | 不必 | 按引用传递，读改写 |
| `out` | 不必 | 必须 | 多个返回值 |
| `in` | 必须 | 不能（只读） | 按引用传递只读，避免大结构体拷贝 |

**加分点**：`ref`/`in` 对**大 struct** 传参可避免拷贝提升性能；`in` 常用于性能敏感路径；值类型按引用传递后对参数赋值会修改调用方变量。


**代码示例：**

```csharp
void Add(ref int n) => n += 10;          // ref：调用前必须初始化，方法内可读写
void TryParse(string s, out int n)       // out：调用前不必初始化，方法内必须赋值
{
    n = int.Parse(s);
}
void Read(in Point p)                    // in：只读引用，避免大结构体拷贝
{
    // p.X = 1; // 编译错误：in 参数只读
    Console.WriteLine(p.X);
}
struct Point { public int X; public int Y; }

int v = 1;
Add(ref v);                 // v == 11
TryParse("123", out int r); // r == 123
Read(new Point { X = 1, Y = 2 });
```

### 1.5 可空类型（`Nullable<T>` / `int?`）是什么？

- 值类型不能为 `null`，可空类型允许值类型表示"有值/无值"（如数据库空列）。
- 本质是 `Nullable<T>` 结构体，包含 `HasValue` 与 `Value` 两个属性。
- 运算符：`??`（空合并）、`?.`（空条件访问）、`??=`（空合并赋值）。
- 与数据库、JSON 交互时很常用；`null` 与 `0` 语义要区分清楚。


**代码示例：**

```csharp
int? age = null;              // 本质是 Nullable<int>，含 HasValue / Value
if (age.HasValue) Console.WriteLine(age.Value);

int real = age ?? 0;          // ?? 空合并：null 时用 0
age ??= 25;                   // ??= 空合并赋值：为 null 才赋值
int len = name?.Length ?? 0;  // ?. 空条件访问：null 安全

// null 与 0 语义不同：数据库中"未填写"与"填了 0"要区分
```

### 1.6 抽象类（abstract class）与接口（interface）的区别？（必问）

**要点：**
- **抽象类**：可以有**字段、构造函数、方法实现**；一个类**只能继承一个**抽象类；表示"是什么"（is-a）。
- **接口**：C# 8+ 可以有默认实现，但**不能有字段**（可有静态成员/属性）；一个类**可实现多个**接口；表示"能做什么"（can-do）。
- **选择**：有共享状态/实现逻辑用抽象类；定义契约、多行为组合用接口。
- 抽象类需 `abstract` 关键字，可含抽象方法（子类必须实现）；接口方法默认 `public`。


**代码示例：**

```csharp
// 抽象类：可含字段/构造函数/实现，表示"是什么"（is-a），只能单继承
abstract class Animal
{
    protected string Name = "";
    public void Eat() => Console.WriteLine("吃");   // 具体实现
    public abstract void Speak();                    // 抽象方法，子类必须实现
}

// 接口：定义契约，表示"能做什么"（can-do），可多实现
interface IFlyable { void Fly(); }
interface ISwimmable { void Swim(); }

class Duck : Animal, IFlyable, ISwimmable
{
    public override void Speak() => Console.WriteLine("嘎嘎");
    public void Fly() { }
    public void Swim() { }
}
```

### 1.7 方法重载（Overload）与重写（Override）的区别？

- **重载**：同一个类中，方法名相同、**参数列表不同**，编译期决定，与返回类型无关。
- **重写**：子类用 `override` 重新实现父类的 `virtual`/`abstract` 方法，签名一致，**运行时多态**。
- `new` 关键字可隐藏父类方法（非多态），要区分。


**代码示例：**

```csharp
class Calculator
{
    // 重载 Overload：同名方法、参数列表不同，编译期决定调用哪个
    public int Add(int a, int b) => a + b;
    public double Add(double a, double b) => a + b;
    public int Add(int a, int b, int c) => a + b + c;
}

class Base { public virtual void Show() => Console.WriteLine("Base"); }
class Derived : Base
{
    // 重写 Override：运行时多态，签名一致
    public override void Show() => Console.WriteLine("Derived");
}
```

### 1.8 `virtual`、`override`、`abstract`、`sealed`、`static` 的含义？

- `virtual`：允许子类重写的方法。
- `override`：子类重写父类虚方法/抽象方法。
- `abstract`：抽象成员/类，不能实例化，派生类必须实现。
- `sealed`：密封类不可继承；密封方法不可再重写。
- `static`：静态成员属于类型本身，所有实例共享；静态类不能实例化、不能被继承。


**代码示例：**

```csharp
class Base
{
    public virtual void V() { }     // virtual：允许子类重写
    public static int Count;        // static：属于类型本身，所有实例共享
}

abstract class Abs { public abstract void Must(); }  // abstract：子类必须实现

class Derived : Base
{
    public override void V() { }    // override：重写父类虚方法
    public sealed override string ToString() => "x"; // sealed 方法：不可再被重写
}

sealed class Final { }              // sealed 类：不可被继承
```

### 1.9 泛型（Generic）是什么？有什么好处？什么是泛型约束？

**要点：**
- 泛型把**类型参数化**，在编译期确定类型，避免装箱、保证类型安全、代码复用。
- 泛型约束：`where T : class / struct / new() / 基类 / 接口 / notnull / unmanaged` 等。
- 例子：`List<T>`、`Dictionary<K,V>`、`IEnumerable<T>`。
- 泛型类、泛型方法、泛型接口；协变 `out T`（`IEnumerable<out T>`）与逆变 `in T`（`IComparer<in T>`）。


**代码示例：**

```csharp
// 泛型方法：类型参数化，编译期确定类型，避免装箱、保证类型安全
T Max<T>(T a, T b) where T : IComparable<T>
{
    return a.CompareTo(b) > 0 ? a : b;
}

int m1 = Max(3, 5);
string m2 = Max("a", "b");

// 泛型约束：where T : class/struct/new()/基类/接口/notnull/unmanaged
public class Repository<T> where T : class, new() { }
```

### 1.10 什么是委托（delegate）？多播委托是什么？

- 委托是**方法引用的类型**，相当于类型安全的函数指针；可把方法当作参数传递。
- 内置委托：`Action`（无返回值）、`Func<T>`（有返回值）、`Predicate<T>`（返回 bool）。
- **多播委托**：`+=` 链式绑定多个方法，调用时按顺序执行；`-=` 移除；返回值取最后一个方法的返回值。
- 匿名方法、Lambda 表达式是委托的简洁写法。


**代码示例：**

```csharp
delegate int MathOp(int x, int y);

int Add(int a, int b) => a + b;
int Mul(int a, int b) => a * b;

MathOp op = Add;
Console.WriteLine(op(2, 3));        // 5

// 多播委托：+= 链式绑定多个方法，按顺序执行；返回值取最后一个
op += Mul;
op(2, 3);                           // 先 Add 再 Mul

// 内置委托：Action 无返回值，Func 有返回值
Func<int, int, int> f = (a, b) => a + b;
Action<string> print = s => Console.WriteLine(s);
```

### 1.11 事件（event）与委托（delegate）的关系和区别？（高频）

**要点：**
- 事件**基于委托**，是委托的封装，用于发布/订阅（观察者模式）。
- **区别**：
  1. 事件只能在**声明它的类内部触发**（`Invoke`），外部只能 `+=`/`-=` 订阅；委托可任意赋值调用。
  2. 事件字段默认 `private` 的委托实例 + 外部订阅接口，安全性更高。
  3. 事件强调"状态变化通知"，委托强调"回调抽象"。
- 标准事件模式：`EventHandler<TEventArgs>` + `sender` 参数。


**代码示例：**

```csharp
class Button
{
    // 事件基于委托：外部只能 += / -= 订阅，不能从类外 Invoke
    public event EventHandler? Click;

    public void OnClick()
    {
        Click?.Invoke(this, EventArgs.Empty);  // 只能在声明类内部触发
    }
}

var btn = new Button();
btn.Click += (s, e) => Console.WriteLine("按钮被点击");
btn.OnClick();  // 输出：按钮被点击
```

### 1.12 什么是 Lambda 表达式？闭包（Closure）是什么？

- Lambda：`(x) => x * 2`，匿名函数简洁语法，可转为委托或表达式树。
- **闭包**：Lambda 捕获外部变量，该变量被提升为编译器生成的类的字段，生命周期延长到委托被释放。
- **陷阱**：`for` 循环中捕获循环变量——旧版本 C# 捕获的是同一个变量，C# 5 起 `foreach` 每次迭代的变量是新的；`for` 循环的索引仍需要注意。


**代码示例：**

```csharp
// Lambda：匿名函数的简洁语法
Func<int, int> square = x => x * x;

// 闭包：Lambda 捕获外部变量，变量生命周期被延长
int factor = 10;
Func<int, int> multiply = x => x * factor;  // 捕获 factor
Console.WriteLine(multiply(5));             // 50

// 陷阱：for 循环捕获循环变量（旧版 C# 捕获同一个变量）
var actions = new List<Action>();
for (int i = 0; i < 3; i++)
    actions.Add(() => Console.Write(i));    // 输出 333（C# 5 前）；C# 5+ 的 foreach 每次是新变量
foreach (var a in actions) a();
```

### 1.13 什么是 LINQ？它的工作原理是什么？

**要点：**
- LINQ（Language Integrated Query）：统一的声明式查询语法，可查询集合、数据库、XML 等数据源。
- 两类语法：查询语法（`from...where...select`）与方法语法（`.Where().Select()`）。
- 核心机制：
  1. **延迟执行（Deferred Execution）**：查询定义时不执行，遍历（`foreach`、`ToList()` 等）时才执行；
  2. **提供程序模型**：LINQ to Objects（`IEnumerable<T>`）、LINQ to Entities（`IQueryable<T>` + EF）、LINQ to XML；
  3. 链式组合：每次调用返回新的查询对象。


**代码示例：**

```csharp
var nums = new[] { 1, 2, 3, 4, 5, 6 };

// 方法语法：链式组合
var even = nums.Where(n => n % 2 == 0).Select(n => n * 10);

// 查询语法
var even2 = from n in nums
            where n % 2 == 0
            select n * 10;

// 延迟执行：定义查询不执行，遍历/物化时才执行
var query = nums.Where(n => { Console.WriteLine($"判断 {n}"); return n > 2; });
Console.WriteLine("尚未执行");          // 先打印这行
var list = query.ToList();              // 此时才真正遍历
```

### 1.14 `IEnumerable<T>` 与 `IQueryable<T>` 的区别？（高频）

| 维度 | `IEnumerable<T>` | `IQueryable<T>` |
|---|---|---|
| 命名空间 | System.Collections | System.Linq |
| 执行方式 | 内存中遍历（LINQ to Objects） | 可被 Provider 翻译成 SQL（LINQ to Entities） |
| 延迟执行 | 是 | 是（更强） |
| 适用 | 内存集合 | 数据库查询（EF Core） |
| 性能 | 会把全部数据拉进内存 | 查询在数据库端执行，只返回结果 |

**加分点**：EF Core 中 `IQueryable` 上组合条件不会立即执行，`ToList()` 才真正发 SQL；先 `ToList()` 再 `Where` 会把过滤放到内存中（错误用法）。


**代码示例：**

```csharp
// IEnumerable：内存中遍历
IEnumerable<Product> all = GetProducts();          // 已全部在内存
var filtered = all.Where(p => p.Price > 100);      // 内存中过滤

// IQueryable：可被 Provider 翻译成 SQL（EF Core）
IQueryable<Product> query = _context.Products;     // 只是表达式树
var result = query.Where(p => p.Price > 100)
                  .OrderBy(p => p.Name)
                  .ToList();                        // ToList() 才发 SQL

// 错误用法：先 ToList 再 Where，过滤被放到内存（全表拉取）
var wrong = _context.Products.ToList().Where(p => p.Price > 100);
```

### 1.15 什么是扩展方法（Extension Methods）？

- 静态类中的静态方法，第一个参数加 `this` 关键字，即可像实例方法一样调用。
- 不能访问被扩展类型的私有成员；本质上还是静态方法调用。
- 经典例子：LINQ 的 `Where`/`Select`、`string.IsNullOrEmpty` 扩展。
- 命名空间必须 `using` 后才能使用；若与实例方法同名，实例方法优先。


**代码示例：**

```csharp
public static class StringExtensions
{
    // 静态类中的静态方法，第一个参数加 this
    public static bool IsNullOrWhiteSpace(this string? s)
        => string.IsNullOrWhiteSpace(s);

    public static string Reverse(this string s)
        => new(s.Reverse().ToArray());
}

// 像实例方法一样调用
string name = "abc";
bool empty = name.IsNullOrWhiteSpace();   // false
Console.WriteLine(name.Reverse());        // cba
```

### 1.16 什么是匿名类型（Anonymous Type）？

- 用 `new { Name = "x", Age = 18 }` 创建，编译器自动生成类型，属性只读。
- 只能配合 `var` 使用；常用于 LINQ 投影（`Select(x => new { ... })`）。
- C# 7+ 支持元组（`(int Id, string Name)`）作为替代，更轻量。


**代码示例：**

```csharp
// 匿名类型：编译器自动生成类型，属性只读，只能配合 var
var person = new { Name = "张三", Age = 18 };
Console.WriteLine($"{person.Name} {person.Age}");

// 常用于 LINQ 投影
var users = new[] { new User { Id = 1, Name = "a" }, new User { Id = 2, Name = "b" } };
var dto = users.Select(u => new { u.Id, UpperName = u.Name.ToUpper() });

// C# 7+ 元组替代：更轻量，支持解构
(int Id, string Name) t = (1, "张三");
var (id, name) = t;
class User { public int Id { get; set; } public string Name { get; set; } = ""; }
```

### 1.17 `var`、`dynamic`、`object` 的区别？

- `var`：编译期**类型推断**，变量类型固定，只是不用显式写出。
- `object`：所有类型的基类，编译期类型检查存在（需要强转才能用具体成员），使用值类型会**装箱**。
- `dynamic`：编译期**不检查**，运行时动态解析（DLR），性能更低，适合 COM 互操作、反射、动态 JSON；错误推迟到运行时。
- 结论：能用 `var` 用 `var`；能用强类型不用 `object`；`dynamic` 慎用。


**代码示例：**

```csharp
var num = 10;               // 编译期推断为 int，类型固定
// num = "str";             // 编译错误：类型已固定

object obj = 10;            // object：使用值类型会装箱；调用成员需强转
int n = (int)obj;

dynamic d = 10;             // dynamic：编译期不检查，运行时解析（DLR）
d = "hello";
Console.WriteLine(d.Length); // 编译通过，运行时才解析；错误也推迟到运行时
```

### 1.18 什么是异常处理？`try/catch/finally` 的执行顺序？`throw` vs `throw ex`？

- **执行顺序**：`try` → 发生异常进 `catch` → 无论是否异常都执行 `finally`（除非进程终止/`Environment.FailFast`）。
- **`throw;`**：保留原始异常堆栈；**`throw ex;`**：重置堆栈为当前抛出点（**不推荐**，会丢失原始调用链信息）。
- **自定义异常**：继承 `Exception`，命名以 `Exception` 结尾，建议实现 3 个构造函数。
- **最佳实践**：只捕获能处理的异常；用 `catch (SpecificException)`；别捕获 `Exception` 后静默吞掉；`finally` 中释放资源或用 `using`。


**代码示例：**

```csharp
try
{
    int x = int.Parse("abc");
}
catch (FormatException ex)
{
    Console.WriteLine("格式错误");
    // throw;      // 保留原始堆栈，推荐
    // throw ex;   // 重置堆栈到当前抛出点，丢失原始调用链，不推荐
}
catch (Exception)
{
    // 只捕获能处理的异常，不要静默吞掉
}
finally
{
    Console.WriteLine("无论是否异常都执行，用于释放资源");
}
```

### 1.19 什么是 `using` 语句？`IDisposable` 是什么？

- `using` 语句在代码块结束时自动调用 `Dispose()`，用于确定性释放非托管资源（文件、连接、Socket）。
- `IDisposable`：实现 `Dispose()` 方法；`using var x = ...`（C# 8+）在作用域结束时释放。
- **陷阱**：`using` 不能保证进程崩溃时执行；`Dispose` 不应抛异常；注意与 `GC.SuppressFinalize(this)` 配合的 Dispose 模式。


**代码示例：**

```csharp
// using 语句：代码块结束时自动调用 Dispose
using (var fs = new FileStream("a.txt", FileMode.Open))
{
    // 使用 fs
}   // 此处自动释放

// C# 8+：using 声明，作用域结束时释放
using var reader = new StreamReader("a.txt");
string line = reader.ReadLine()!;

// 自定义可释放类型
class MyResource : IDisposable
{
    public void Dispose() => Console.WriteLine("已释放");
}
```

### 1.20 什么是属性（Property）？自动属性与 `init` 只读属性？

- 属性是字段的封装，包含 `get`/`set` 访问器，可在其中加验证逻辑。
- 自动属性：`public int Age { get; set; }`，编译器生成后备字段。
- C# 9+ `init`：属性只能在**对象初始化器/构造函数**中赋值（不可变对象友好）。
- `required`（C# 11）：标记必填属性，编译期强制初始化。


**代码示例：**

```csharp
class Person
{
    private int _age;
    public int Age
    {
        get => _age;
        set { if (value < 0) throw new ArgumentOutOfRangeException(); _age = value; }
    }

    public string Name { get; set; } = "";   // 自动属性

    public DateTime CreatedAt { get; init; } // C# 9+ init：只能在初始化器/构造函数赋值

    public required string IdCard { get; set; } // C# 11 required：编译期强制初始化
}

var p = new Person { Age = 18, CreatedAt = DateTime.Now, IdCard = "110..." };
// p.CreatedAt = DateTime.Now; // 编译错误：init 只读
```

### 1.21 什么是索引器（Indexer）？

- 让对象像数组一样用 `obj[index]` 访问：`public T this[int i] { get; set; }`。
- 可用于集合类、字典包装类。


**代码示例：**

```csharp
class ShoppingCart
{
    private readonly Dictionary<string, decimal> _items = new();

    public decimal this[string name]              // 索引器：obj["牛奶"]
    {
        get => _items.TryGetValue(name, out var v) ? v : 0m;
        set => _items[name] = value;
    }
}

var cart = new ShoppingCart();
cart["牛奶"] = 12.5m;
Console.WriteLine(cart["牛奶"]);   // 12.5
```

### 1.22 什么是运算符重载？

- 用 `public static` 方法重载运算符（`+`、`-`、`==`、`<` 等）。
- 重载 `==` 时必须重载 `Equals` 和 `GetHashCode`。
- 常用于自定义数值类型、`Vector` 等。


**代码示例：**

```csharp
struct Vector
{
    public double X, Y;
    public Vector(double x, double y) { X = x; Y = y; }

    public static Vector operator +(Vector a, Vector b)
        => new(a.X + b.X, a.Y + b.Y);

    public static bool operator ==(Vector a, Vector b)
        => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Vector a, Vector b) => !(a == b);

    // 重载 == 必须同时重写 Equals 和 GetHashCode
    public override bool Equals(object? obj) => obj is Vector v && this == v;
    public override int GetHashCode() => HashCode.Combine(X, Y);
}

var v = new Vector(1, 2) + new Vector(3, 4);  // (4, 6)
```

### 1.23 `string` 相等比较：`==`、`Equals`、`ReferenceEquals`？

- `==`：`string` 类型重载后比较**内容**（值语义）。
- `Equals`：默认比较内容（`string` 重写后）。
- `ReferenceEquals`：比较引用地址。
- 区分大小写：默认区分；可用 `StringComparison.OrdinalIgnoreCase` / `StringComparer`。


**代码示例：**

```csharp
string a = "hello";
string b = "hello";          // 同一字面量，运行时驻留（Interned），引用相同
string c = new string('h', 1) + "ello";  // 动态拼接，不驻留

Console.WriteLine(a == b);             // True  内容比较（string 重载 ==）
Console.WriteLine(a == c);             // True  内容比较
Console.WriteLine(ReferenceEquals(a, c)); // False 引用不同

// 忽略大小写比较
bool eq = string.Equals(a, "HELLO", StringComparison.OrdinalIgnoreCase); // True
```

### 1.24 什么是常量（const）与只读字段（readonly）的区别？

| 维度 | `const` | `readonly` |
|---|---|---|
| 赋值时机 | 声明时必须赋值（编译期常量） | 声明时或构造函数中 |
| 类型限制 | 只能是基元类型/string/enum | 任意类型 |
| 存储 | 编译期替换，属于类型 | 运行时赋值，实例字段 |
| static | 隐式 static | 实例成员（可 static readonly） |


**代码示例：**

```csharp
class Config
{
    public const int MaxRetry = 3;          // const：编译期常量，只能是基元类型/string/enum
    public readonly string Env;             // readonly：声明时或构造函数中赋值，任意类型

    public Config(string env) { Env = env; }
    public static readonly DateTime Startup = DateTime.Now; // static readonly
}

Console.WriteLine(Config.MaxRetry);   // 编译期直接替换为 3
```

### 1.25 什么是静态成员？静态类有什么特点？

- 静态成员属于类本身，所有实例共享，类加载时初始化。
- 静态类：不能实例化、不能继承、只能包含静态成员；如 `Math`、`Console`。
- 静态构造函数：类型首次被使用前执行一次，用于静态初始化。
- 静态字段生命周期 = 应用生命周期，**滥用会导致内存长期占用**。


**代码示例：**

```csharp
class Counter
{
    public static int Total;              // 静态字段：属于类本身，所有实例共享
    public static void Reset() => Total = 0;  // 静态方法

    static Counter() => Total = 100;      // 静态构造函数：类型首次使用前执行一次
}

static class MathHelper                       // 静态类：不能实例化、不能继承
{
    public static double Square(double x) => x * x;
}
```

### 1.26 什么是枚举（enum）？`[Flags]` 特性是什么？

- 枚举：一组命名常量，底层默认 `int`。
- `[Flags]`：位标志枚举，值应为 2 的幂，可用 `|` 组合、`&` 判断、`HasFlag()` 检查。
- 枚举转字符串/数字、解析：`Enum.Parse`、`ToString()`、`nameof`。


**代码示例：**

```csharp
enum Color { Red, Green = 5, Blue }        // 底层默认 int

[Flags]
enum Permissions
{
    None = 0,
    Read = 1, Write = 2, Delete = 4, Admin = Read | Write | Delete
}

Permissions p = Permissions.Read | Permissions.Write;  // 用 | 组合
bool canRead = (p & Permissions.Read) != 0;            // 用 & 判断
bool isAdmin = p.HasFlag(Permissions.Admin);           // HasFlag 检查

Console.WriteLine(Enum.Parse<Color>("Red"));           // 字符串转枚举
Console.WriteLine((int)Color.Green);                   // 5
```

### 1.27 什么是结构体（struct）？什么时候用 struct 而不是 class？

- `struct` 是值类型，无继承（可实现接口）、无默认构造函数（C# 10+ 允许显式无参构造）、不应长期存在大对象。
- **用 struct 的场景**：小（≤16~24 字节）、不可变、短生命周期、不频繁装箱；如坐标、颜色、`Guid`。
- **用 class 的场景**：较大、需要多态/继承、作为引用共享。
- **陷阱**：struct 不能为 null（除非可空）；按值传递副本；修改副本不影响原对象。


**代码示例：**

```csharp
struct Point { public int X; public int Y; }   // 值类型：小、不可变、短生命周期

var p1 = new Point { X = 1, Y = 2 };
var p2 = p1;             // 复制整个结构（副本）
p2.X = 99;
Console.WriteLine(p1.X); // 1，修改副本不影响原对象

// 不要用 struct 的场景：需要继承/多态、对象较大、长期存在于集合中反复装箱
class Order { public long Id; }   // 引用类型：共享引用
```

### 1.28 什么是元组（Tuple）与值元组（ValueTuple）？

- `Tuple`（引用类型）：`Tuple.Create(1, "a")`，成员为 `Item1`...，可空，性能差。
- `ValueTuple`（值类型）：`(int Id, string Name) p = (1, "a")`，支持命名成员、解构 `var (id, name) = p`，性能更好。
- 现代代码用 `ValueTuple`；常用于多返回值。


**代码示例：**

```csharp
// Tuple（引用类型，旧）：成员 Item1/Item2，可空
var t = Tuple.Create(1, "a");
Console.WriteLine(t.Item1);

// ValueTuple（值类型，现代推荐）：命名成员、支持解构、性能更好
(int Id, string Name) p = (1, "张三");
Console.WriteLine(p.Name);

var (id, name) = p;    // 解构

// 方法多返回值常用
(int, string) GetUser() => (1, "李四");
```

### 1.29 什么是 `switch` 表达式与模式匹配？

- C# 8+ switch 表达式：`var result = x switch { 1 => "a", _ => "b" };`
- 模式匹配：类型模式 `x is int i`、属性模式、位置模式、关系模式 `> 10`、逻辑模式 `and/or/not`。
- 常量模式、空检查 `x is null`。


**代码示例：**

```csharp
// switch 表达式（C# 8+）
string GetDay(int n) => n switch
{
    1 => "周一",
    7 => "周日",
    _ => "其他"          // 默认分支
};

// 模式匹配：类型模式、属性模式、关系模式、逻辑模式
string Describe(object o) => o switch
{
    int i when i > 0 => $"正数 {i}",
    int i => $"非正数 {i}",
    string s => $"字符串 {s.Length}",
    null => "null",
    _ => "未知"
};

// is 类型模式（C# 7+）
if (o is int i) Console.WriteLine(i);
```

### 1.30 什么是类型转换？`as`、`is`、强制转换的区别？

- `is`：类型检查，返回 bool；C# 7+ 可 `is Type v` 同时声明变量（模式匹配）。
- `as`：安全转换，失败返回 null（**仅限引用类型**）。
- 强制转换 `(Type)x`：失败抛 `InvalidCastException`。
- `Convert`：类型转换 + 值转换（如字符串转数字）。


**代码示例：**

```csharp
object obj = "hello";

bool isStr = obj is string;                    // is：类型检查，返回 bool
if (obj is string s) Console.WriteLine(s);     // is 模式匹配，同时声明变量

string? str = obj as string;                   // as：安全转换，失败返回 null（仅引用类型）
int? num = obj as int?;                        // 失败为 null，不抛异常

// 强制转换：失败抛 InvalidCastException
// int n = (int)obj;                           // 抛异常

// Convert：类型 + 值转换（如字符串转数字）
int parsed = Convert.ToInt32("123");
```

### 1.31 数组与集合的区别？常用集合类的特点？

- 数组：固定长度、连续内存、`int[]`；性能好但不可变长。
- `List<T>`：动态扩容数组。
- `Dictionary<K,V>`：哈希表，O(1) 查找。
- `HashSet<T>`：无重复集合，O(1) 去重/判断。
- `LinkedList<T>`：链表，插入删除快。
- `Queue<T>` / `Stack<T>`：FIFO / LIFO。
- `SortedDictionary` / `SortedList`：有序。
- **线程安全**：`ConcurrentDictionary`、`ConcurrentQueue`、`BlockingCollection`。


**代码示例：**

```csharp
int[] arr = new int[3];               // 数组：固定长度、连续内存
List<int> list = new() { 1, 2, 3 };   // 动态扩容
list.Add(4);

var dict = new Dictionary<string, int> { ["a"] = 1 };  // 哈希表，O(1) 查找
var set = new HashSet<int> { 1, 2, 3 };                 // 去重，O(1)
var queue = new Queue<int>();                           // FIFO
var stack = new Stack<int>();                           // LIFO
var ll = new LinkedList<int>();                         // 插入删除快

// 线程安全集合
var concurrent = new ConcurrentDictionary<string, int>();
```

### 1.32 什么是 `Object` 的四个方法？

- `ToString()`、`Equals(object)`、`GetHashCode()`、`GetType()`。
- `Equals` 与 `GetHashCode` 约定：相等对象必须哈希一致；重写 `Equals` 必须重写 `GetHashCode`（否则字典/哈希集合行为错误）。


**代码示例：**

```csharp
class Person
{
    public string Name { get; set; } = "";
    public int Age { get; set; }

    public override string ToString() => $"{Name}-{Age}";

    // 约定：Equals 相等则 GetHashCode 必须一致（否则字典/HashSet 行为错误）
    public override bool Equals(object? obj)
        => obj is Person p && p.Name == Name && p.Age == Age;

    public override int GetHashCode() => HashCode.Combine(Name, Age);
}

var p = new Person { Name = "a", Age = 1 };
Type t = p.GetType();        // 获取类型信息
```

### 1.33 什么是不可变对象（Immutable）？有什么好处？

- 创建后状态不可变（只读字段、`init`、`record`）。
- 好处：线程安全、可安全共享、可缓存、适合函数式编程与并发。
- 缺点：频繁修改会产生新对象，注意性能。


**代码示例：**

```csharp
// 不可变：创建后状态不可变 -> 线程安全、可安全共享、可缓存
class ImmutableConfig
{
    public ImmutableConfig(int timeout, string url)
    {
        Timeout = timeout;
        Url = url;
    }
    public int Timeout { get; }      // 只读属性
    public string Url { get; }
}

// record 自带不可变语义 + with 非破坏性修改
var cfg = new ImmutableConfig(30, "https://x.com");
// cfg.Timeout = 60;                // 编译错误
```

### 1.34 什么是 `record`？与 `class` 的区别？（高频，C# 9+）

**要点：**
- `record` 是引用类型，但提供**值相等性**（基于属性逐个比较）和 `with` 表达式（非破坏性修改）。
- 编译器自动生成：`Equals`、`GetHashCode`、`ToString()`、`==`/`!=`、`Deconstruct`、克隆保护。
- `record struct`（C# 10+）：值类型记录。
- **使用场景**：DTO、不可变数据模型、值对象。
- **注意**：性能敏感场景 record 的相等性比较有开销；继承场景中相等性语义要小心。


**代码示例：**

```csharp
record Person(string Name, int Age);   // 编译器自动生成 Equals/GetHashCode/ToString/Deconstruct

var p1 = new Person("张三", 18);
var p2 = new Person("张三", 18);
Console.WriteLine(p1 == p2);        // True：值相等性（逐个属性比较）

var p3 = p1 with { Age = 20 };      // with：非破坏性修改，创建新对象
Console.WriteLine(p1.Age);          // 18，原对象不变
Console.WriteLine(p3);              // Person { Name = 张三, Age = 20 }

// record struct（C# 10+）：值类型记录
readonly record struct Point(int X, int Y);
```

### 1.35 什么是主构造函数（Primary Constructor）？（C# 12 / C# 13）

- C# 12：`public class OrderService(IOrderRepository repo) { ... }`，参数在类体内可用，简化 DI 构造。
- C# 13 增强：支持 `readonly` 修饰参数、隐式字段绑定，class/struct/record 语义有差异（record 参数自动生成属性参与值相等性比较；class 不自动生成属性，需自行赋值）。
- **注意**：C# 13 主构造参数默认生成隐式字段（若未显式使用）；不要重复声明字段导致编译警告。


**代码示例：**

```csharp
// C# 12：主构造函数，参数在类体内可直接使用，简化 DI
public class OrderService(IOrderRepository repo, ILogger<OrderService> logger)
{
    public async Task<Order> GetAsync(long id)
    {
        logger.LogInformation("查询订单 {Id}", id);   // 直接使用主构造参数
        return await repo.GetByIdAsync(id);
    }
}

// C# 13 增强：class 主构造参数默认生成隐式字段（未显式使用到时）
public class Greeter(string name)   // 隐式生成私有字段 _name
{
    public string Hello() => $"你好，{name}";
}
```

### 1.36 什么是 `Span<T>` / `Memory<T>`？（高级）

- `Span<T>`：栈上分配的**连续内存切片**，无堆分配、无装箱，性能极高；不能跨越 `await`（ref struct）。
- `Memory<T>`：托管堆上的可变长度内存，可异步使用。
- 用于高性能解析、字符串处理、避免复制；`ReadOnlySpan<char>` 常用于字符串切片。


**代码示例：**

```csharp
// Span<T>：连续内存切片，零堆分配、无拷贝；ref struct，不能跨 await
string s = "123,456,789";
ReadOnlySpan<char> span = s.AsSpan();
int idx = span.IndexOf(',');
ReadOnlySpan<char> first = span[..idx];     // 切片无分配
Console.WriteLine(first.ToString());        // 123

int[] arr = { 1, 2, 3, 4, 5 };
Span<int> slice = arr.AsSpan(1, 3);         // 指向原数组切片
slice[0] = 99;                              // 直接修改原数组

// Memory<T>：托管堆上的可变内存，可异步使用（await 场景用 Memory）
Memory<char> mem = s.AsMemory();
```

### 1.37 什么是 `yield return`？

- 迭代器：用 `yield return` 逐个产出元素，**惰性求值**——每次 `MoveNext()` 才执行到下一个 `yield`。
- 优点：不一次性生成全部数据，节省内存（大集合/流式处理）。
- 注意：迭代器方法中不能有 `ref`/`out` 参数；异常在遍历时抛出。


**代码示例：**

```csharp
// 惰性求值：每次 MoveNext() 才执行到下一个 yield，不一次性生成全部
IEnumerable<int> GetNumbers()
{
    for (int i = 0; i < 1_000_000; i++)
    {
        yield return i;      // 逐个产出
    }
}

foreach (var n in GetNumbers().Take(3))  // 只迭代前 3 个，不会生成 100 万个
{
    Console.WriteLine(n);
}

// 注意：迭代器方法中不能有 ref/out 参数；异常在遍历时才抛出
```

### 1.38 什么是命名空间（namespace）与 `using`？

- 命名空间组织类型、避免命名冲突；`using` 导入命名空间。
- `using static`（C# 6+）：直接引入静态成员。
- 全局 using（C# 10+）：`global using` 全项目生效。
- 文件级命名空间（C# 10+）：简化缩进。


**代码示例：**

```csharp
// C# 10+：文件级命名空间，简化缩进
namespace MyApp.Models;

// using 导入命名空间
using System;
using System.Collections.Generic;
using static System.Math;          // using static：直接引入静态成员

global using System.Text;          // C# 10+：全局 using，整个项目生效（写在任意文件顶部）

public class Demo
{
    public double Calc() => Sqrt(16);   // 直接调用 Math.Sqrt
}
```

### 1.39 C# 编译过程：源代码如何变成可执行程序？

1. 源代码 → 编译器（Roslyn）→ **IL（中间语言）** + 元数据，打包为程序集（.dll/.exe）；
2. 运行时 CLR 加载程序集 → **JIT 编译**成机器码 → 执行；
3. 可选：AOT（Native AOT，.NET 7+）提前编译为原生代码，启动更快、体积小，但反射/动态代码受限。

### 1.40 `#if` 预处理器指令是做什么的？

- 条件编译：`#if DEBUG ... #endif`、`#define`、`[Conditional("DEBUG")]`。
- 用于调试代码、平台差异代码；注意过度使用会降低可读性。


**代码示例：**

```csharp
#define TRACE_MODE   // 也可在 csproj 的 DefineConstants 中配置

public class Logger
{
    public static void Debug(string msg)
    {
#if DEBUG
        Console.WriteLine($"[DEBUG] {msg}");      // 仅 DEBUG 构建包含
#elif TRACE_MODE
        Console.WriteLine($"[TRACE] {msg}");
#else
        // 生产环境：不输出
#endif
    }
}

// 等价实现：更整洁的 [Conditional("DEBUG")]
[Conditional("DEBUG")]
public static void Log(string msg) => Console.WriteLine(msg);
```

---

<a id="c2"></a>

## 二、C# 进阶与 .NET 运行时

### 2.1 什么是 CLR？它负责什么？

**要点：**
- CLR（Common Language Runtime，公共语言运行时）：.NET 的执行引擎，所有 .NET 语言共享。
- 核心职责：
  1. **JIT 编译**：把 IL 编译为机器码；
  2. **垃圾回收（GC）**：自动管理托管内存；
  3. **类型安全**：运行时类型检查，防止非法类型转换/内存访问；
  4. 异常处理、线程管理、安全、反射、跨语言互操作。
- 对比：Java 的 JVM 类似，但 CLR 设计为多语言（C#/VB/F#）共享。

### 2.2 什么是 JIT 和 AOT？各有什么优缺点？

- **JIT（Just-In-Time）**：运行时把 IL 编译成机器码，首次调用有编译开销；可针对当前机器做优化；支持反射/动态代码。
- **AOT（Ahead-Of-Time，Native AOT）**：发布时直接编译成本机二进制。优点：启动快、内存小、无需运行时 JIT；缺点：不支持动态加载/部分反射、发布慢。
- .NET 8+ 的 `PublishAot=true` 适合无服务器、CLI 工具等场景。
- **分层编译（Tiered Compilation）**：先用低优化层快速启动，热点方法再高优化，兼顾启动与峰值性能。

### 2.3 垃圾回收（GC）的工作原理？（必问）

**要点：**
1. **分代回收**：分 0 代（新生）、1 代、2 代（老生）；新对象在 0 代，存活对象晋升。新生代回收频繁、开销小；大对象堆（LOH，≥85KB）不压缩、随 2 代回收。
2. **标记（Mark）**：从根（静态字段、线程栈、寄存器、GC Handle）出发标记可达对象。
3. **清除（Sweep）/ 压缩（Compact）**：回收不可达对象；可移动对象压缩堆消除碎片（LOH 默认不压缩）。
4. **终结器（Finalizer）**：有终结器的对象进终结队列，由终结线程异步执行（`~Finalizer`），对象复活一次再被回收。
5. **触发时机**：0 代满、显式 `GC.Collect()`（不推荐）、内存压力、进程退出。

**加分点**：`IDisposable` + `using` 是**确定性释放**，与 GC 无关；`GC.SuppressFinalize` 避免重复终结开销。

### 2.4 什么是托管堆？栈和堆的区别？

- **线程栈**：存储方法调用帧、局部变量（值类型）、引用；自动管理，方法结束即释放，容量小（默认 1MB）。
- **托管堆**：存储对象实例（引用类型）；由 GC 管理，分代分配，可大于栈。
- 引用类型的引用变量在栈上，对象在堆上；值类型嵌套在引用类型内时随对象放堆上。

### 2.5 什么是反射（Reflection）？有什么性能影响？

- 反射：运行时**动态查看和操作类型元数据**——获取类型、创建实例、调用方法、读写字段属性、加载程序集。
- 常见用途：ORM（EF 映射）、DI 容器、序列化、插件系统、`Attribute` 处理。
- **性能影响**：比直接调用慢（类型信息运行时解析、额外分配），高频路径慎用；可用缓存（`Delegate.CreateDelegate`、表达式树编译、`Activator.CreateInstance` 缓存、源生成器）优化。


**代码示例：**

```csharp
// 运行时动态查看类型元数据、创建实例、调用方法
Type t = typeof(Order);
Console.WriteLine(t.Name);                       // Order

// 创建实例
object? obj = Activator.CreateInstance(t);

// 调用方法（高频路径慎用：性能远低于直接调用）
var method = t.GetMethod("GetTotal");
decimal? result = (decimal?)method?.Invoke(obj, null);

// 性能优化：把反射方法编译成委托缓存，避免每次反射调用
var compiled = (Func<Order, decimal>)Delegate.CreateDelegate(
    typeof(Func<Order, decimal>), method!);
class Order { public decimal GetTotal() => 100m; }
```

### 2.6 什么是特性（Attribute）？如何自定义？

- 特性是给代码元素（类、方法、属性）附加**元数据**的类，继承 `System.Attribute`。
- 内置：`[Obsolete]`、`[Serializable]`、`[Authorize]`、`[Route]`、`[ApiController]` 等。
- 自定义：`[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]` + 构造函数/命名参数。
- 读取：通过反射 `GetCustomAttribute` / `GetCustomAttributes`。


**代码示例：**

```csharp
// 1. 定义自定义特性
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
class AuthorAttribute : Attribute
{
    public string Name { get; }
    public string? Email { get; set; }        // 命名参数
    public AuthorAttribute(string name) => Name = name;
}

// 2. 使用特性
[Author("张三", Email = "zhangsan@x.com")]
class OrderService { }

// 3. 通过反射读取
var attr = typeof(OrderService).GetCustomAttribute<AuthorAttribute>();
Console.WriteLine($"{attr?.Name} - {attr?.Email}");
```

### 2.7 什么是程序集（Assembly）？强命名程序集？

- 程序集：.NET 部署和版本控制的最小单元（.dll/.exe），包含 IL 代码 + 元数据 + 资源 + 清单。
- 私有程序集：随应用目录部署；共享程序集：GAC（旧 .NET Framework）。
- 强命名：公钥标记 + 版本签名，防止篡改、确保版本唯一；现代 .NET 中 GAC 已不再使用。
- 程序集加载：`Assembly.Load`、依赖解析（默认探测应用目录）。

### 2.8 什么是元数据（Metadata）？

- 程序集中描述类型、成员、引用等信息的结构化数据；反射读取的就是元数据。
- 编译器生成；CLR 用元数据做类型安全校验、加载、互操作。

### 2.9 什么是 AppDomain？.NET Core 还有吗？

- AppDomain：旧 .NET Framework 中进程内的隔离边界（代码隔离、卸载程序集）。
- **.NET Core / .NET 5+ 不再使用 AppDomain**（一个进程一个应用域），程序集不可卸载（AssemblyLoadContext 提供可卸载场景）。
- 隔离方案变为：进程隔离、容器（Docker）、`AssemblyLoadContext`（插件）。

### 2.10 什么是 `Type`？如何获取类型信息？

- `Type` 是类型元数据的运行时表示：`typeof(int)`、`x.GetType()`、`Type.GetType("名称")`。
- 常用：`GetMethods()`、`GetProperties()`、`IsValueType`、`IsInterface`、`GetCustomAttributes()`。


**代码示例：**

```csharp
Type t1 = typeof(int);                 // 编译期已知类型
Type t2 = 42.GetType();                // 从实例获取
Type? t3 = Type.GetType("System.Int32"); // 按名称加载（字符串）

Console.WriteLine(t1.IsValueType);     // True
Console.WriteLine(t1.IsPrimitive);     // True

foreach (var m in typeof(string).GetMethods().Take(5))
    Console.WriteLine(m.Name);
```

### 2.11 什么是序列化？有哪些方式？

- 序列化：对象 → 字节流/字符串（存储或传输）；反序列化逆向。
- 方式：`System.Text.Json`（推荐）、Newtonsoft.Json、XML（`XmlSerializer`）、二进制（BinaryFormatter **已废弃，有安全风险**）、`DataContractSerializer`。
- 注意循环引用、版本兼容、`[JsonIgnore]`、`[JsonPropertyName]`。


**代码示例：**

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    [System.Text.Json.Serialization.JsonIgnore]
    public string Password { get; set; } = "";   // 忽略敏感字段
}

var user = new User { Id = 1, Name = "张三", Password = "secret" };

// System.Text.Json（推荐）
string json = System.Text.Json.JsonSerializer.Serialize(user);
var back = System.Text.Json.JsonSerializer.Deserialize<User>(json);

Console.WriteLine(json);  // {"Id":1,"Name":"张三"} —— Password 被忽略
```

### 2.12 什么是源生成器（Source Generator）？（高级）

- 编译期分析代码并**生成额外 C# 源码**，减少反射开销、提升启动性能。
- 典型应用：`System.Text.Json` 源生成、`LoggerMessage` 高性能日志、`Regex` 源生成（`[GeneratedRegex]`）、依赖注入优化。
- 与反射对比：无运行时开销、AOT 友好。


**代码示例：**

```csharp
// 典型应用1：正则表达式源生成（避免每次运行编译正则）
[GeneratedRegex(@"^\d{11}$")]
private static partial Regex PhoneRegex();

bool ok = PhoneRegex().IsMatch("13800138000");

// 典型应用2：高性能日志源生成（避免装箱与模板解析）
[LoggerMessage(Level = LogLevel.Information, Message = "用户 {UserId} 登录")]
static partial void LogLogin(ILogger logger, int userId);

// 原理：编译期生成 C# 源码，无反射、AOT 友好，启动性能好
```

### 2.13 什么是 `unsafe` 代码和指针？

- `unsafe` 允许使用指针、直接内存操作（`fixed` 固定对象、`stackalloc`）。
- 场景：高性能互操作、图像处理、算法库；需 `<AllowUnsafeBlocks>true`。
- **风险**：破坏类型安全，需严格控制作用域。


**代码示例：**

```csharp
// 需在 csproj 中设置 <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
unsafe static int Sum(int[] arr)
{
    int sum = 0;
    fixed (int* p = arr)        // fixed 固定对象，防止 GC 移动
    {
        for (int i = 0; i < arr.Length; i++)
            sum += *(p + i);    // 指针算术
    }
    return sum;
}

// stackalloc：栈上分配，零 GC
unsafe static void Demo()
{
    Span<int> buf = stackalloc int[16];   // 更安全的替代写法
}
// 风险：破坏类型安全，需严格控制作用域，仅高性能互操作/算法场景使用
```

### 2.14 什么是 `InternalsVisibleTo`？

- 允许指定程序集访问本程序集 `internal` 成员，常用于**单元测试**（测试程序集访问内部实现）。


**代码示例：**

```csharp
// 方式1：AssemblyInfo.cs 中声明
// [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("MyApp.Tests")]

// 方式2：csproj 中（.NET SDK 风格）
// <ItemGroup>
//   <InternalsVisibleTo Include="MyApp.Tests" />
// </ItemGroup>

internal class InternalHelper            // 内部成员
{
    internal static int Calc(int x) => x * 2;
}

// 测试程序集 MyApp.Tests 即可直接访问 InternalHelper（常用于单元测试）
```

### 2.15 什么是 `StringBuilder` 底层原理？字符池（Interning）是什么？

- `StringBuilder` 内部是 `char[]` 缓冲，容量不足时翻倍扩容。
- 字符串驻留（Interning）：编译期/运行时字符串字面量在进程内共享（`String.Intern`），`==` 对驻留字符串比较引用即内容。动态拼接的字符串不驻留。


**代码示例：**

```csharp
// StringBuilder 内部是 char[] 缓冲，容量不足时翻倍扩容
var sb = new StringBuilder(16);          // 预分配容量，减少扩容
for (int i = 0; i < 1000; i++) sb.Append(i);
string result = sb.ToString();

// 字符串驻留（Interning）：字面量在进程内共享
string a = "hello";
string b = "hello";
Console.WriteLine(ReferenceEquals(a, b)); // True：同一驻留字符串

string c = new string('h', 1) + "ello";   // 动态拼接，不驻留
Console.WriteLine(ReferenceEquals(a, c)); // False

string d = string.Intern(c);              // 手动驻留
Console.WriteLine(ReferenceEquals(a, d)); // True
```

### 2.16 什么是 `CultureInfo` 与文化敏感操作？

- 区域文化影响格式化：日期 `ToString()`、数字小数点、货币。
- 服务端统一 `InvariantCulture` 或固定 `CultureInfo`，避免环境差异导致数据错误（如 SQL 日期转换）。


**代码示例：**

```csharp
decimal price = 1234.5m;
DateTime now = new(2026, 1, 2, 15, 30, 0);

// 不同文化下格式化结果不同
Console.WriteLine(price.ToString("C", new CultureInfo("zh-CN")));  // ¥1,234.50
Console.WriteLine(price.ToString("C", new CultureInfo("en-US")));  // $1,234.50
Console.WriteLine(now.ToString(new CultureInfo("zh-CN")));         // 2026/1/2 15:30:00
Console.WriteLine(now.ToString(new CultureInfo("en-US")));         // 1/2/2026 3:30:00 PM

// 服务端统一用 InvariantCulture，避免环境差异导致数据错误
string s = price.ToString(CultureInfo.InvariantCulture);
decimal p = decimal.Parse(s, CultureInfo.InvariantCulture);
```

### 2.17 什么是弱引用（WeakReference）？

- 不阻止 GC 回收目标对象；`WeakReference` + 目标仍可访问（`IsAlive`/`Target`）。
- 用于大缓存：内存不足时缓存对象可被回收，避免内存泄漏。


**代码示例：**

```csharp
// 强引用：GC 无法回收
var strong = new BigObject();

// 弱引用：不阻止 GC 回收目标
var weak = new WeakReference(new BigObject());

// 使用前检查是否还存活
if (weak.IsAlive && weak.Target is BigObject obj)
{
    Console.WriteLine("对象仍存活");
}
else
{
    Console.WriteLine("对象已被 GC 回收");
    weak.Target = new BigObject();   // 重新填充
}

// 用途：大缓存实现——内存不足时缓存对象可被回收，避免内存泄漏
class BigObject { public byte[] Data = new byte[1024 * 1024]; }
```

### 2.18 什么是 `Conditional` 特性？

- `[Conditional("DEBUG")]` 标记的方法在指定符号未定义时**调用点被移除**，方法体仍需编译。
- 比 `#if` 更整洁，常用于调试辅助方法。


**代码示例：**

```csharp
using System.Diagnostics;

class DebugHelper
{
    [Conditional("DEBUG")]          // DEBUG 未定义时，调用点整行被移除
    public static void Trace(string msg) => Console.WriteLine($"[TRACE] {msg}");
}

class Program
{
    static void Main()
    {
        DebugHelper.Trace("开始处理");   // 发布版（无 DEBUG）这行不编译
        Console.WriteLine("业务逻辑");
    }
}

// 与 #if 相比更整洁：调用方无需加条件包裹
```

### 2.19 什么是文化无关字符串比较？

- `string.Compare(a, b, StringComparison.Ordinal)`：按字节序比较，性能高、确定性好。
- 推荐：内部标识符/路径用 `Ordinal`；用户文本排序用 `CurrentCulture`。


**代码示例：**

```csharp
// Ordinal：按字节序比较，性能高、结果确定（内部标识符/路径用）
bool r1 = string.Compare("a", "B", StringComparison.Ordinal) > 0;   // True（大写在前）

// CurrentCulture：按语言排序规则（用户可见文本排序用）
bool r2 = string.Equals("straße", "strasse", StringComparison.CurrentCulture);

// 忽略大小写的确定性比较（用户输入校验常用）
bool r3 = string.Equals("ABC", "abc", StringComparison.OrdinalIgnoreCase); // True
```

### 2.20 什么是 `Null` 传播与空值安全？

- `?.`（null 条件）、`??`（合并）、`??=`（赋值）、`!`（null 容忍运算符，声明"不为空"）。
- 可空引用类型（NRT）：`string?` 表示可能为 null，编译器做流分析警告。


**代码示例：**

```csharp
class Order { public Customer? Customer { get; set; } }
class Customer { public string? Name { get; set; } }

Order? order = GetOrder();      // 可能为 null

// ?. 空条件访问：链上任一环节为 null 则整体为 null，不抛异常
int? len = order?.Customer?.Name?.Length;

// ?? 空合并 / ??= 空合并赋值
string display = order?.Customer?.Name ?? "匿名用户";
int count = order?.Customer?.Name?.Length ?? 0;

// ! 空容忍运算符：声明"这里不可能为 null"（慎用，仅当确实保证非空）
string name = order!.Customer!.Name!;

// 可空引用类型（NRT）：string? 表示可能为 null，编译器做流分析
static Order? GetOrder() => null;
```

---

<a id="c3"></a>

## 三、异步编程与多线程并发

### 3.1 什么是 `async`/`await`？底层是如何工作的？（必问）

**要点：**
- `async` 标记方法允许使用 `await`；`await` 挂起方法，等待的 Task 完成前**释放当前线程**，完成后在上下文（或无上下文）中继续执行后续代码。
- **底层是状态机**：编译器把 async 方法转换为状态机类，每个 `await` 是状态转换点；`Task` 保存状态与结果。
- **关键认知**：`async` 不会创建新线程；I/O 密集操作（网络、文件、DB）本质是异步完成，`await` 时线程被释放回线程池，无线程被阻塞等待。
- **返回值**：`Task`、`Task<T>`、`ValueTask<T>`、`void`（仅限事件处理器）。
- **原则**：async 一路向上（async all the way），避免 `.Result`/`.Wait()` 阻塞。


**代码示例：**

```csharp
// async 不创建新线程；await 挂起时释放当前线程，完成后再继续
static async Task Main()
{
    Console.WriteLine($"线程 {Environment.CurrentManagedThreadId} 开始");

    // I/O 等待期间线程被释放回线程池，不阻塞
    string html = await new HttpClient().GetStringAsync("https://example.com");
    // 完成后在同步上下文（或无上下文）中继续

    Console.WriteLine(html.Length);
}

// 底层是状态机：编译器生成状态机类，每个 await 是一个状态转换点
// 原则：async 一路向上；不要用 .Result/.Wait() 阻塞
static async Task<string> FetchAsync() => await Task.FromResult("data");
```

### 3.2 `Task` 与 `Thread` 的区别？（高频）

| 维度 | `Task` | `Thread` |
|---|---|---|
| 抽象层次 | 高层抽象（表示异步操作） | 底层执行单元 |
| 管理 | CLR/线程池管理 | 手动创建管理 |
| 返回值 | 可返回结果 `Task<T>` | 不能直接返回 |
| 资源 | 复用线程池线程 | 每个线程独立资源（1MB 栈） |
| 取消 | `CancellationToken` 支持 | 不直接支持 |
| 适用 | I/O 与 CPU 密集均可（配合线程池） | 长驻、需要专属线程 |

**加分点**：`Task` 默认在线程池线程上运行；`Task.Run` 把 CPU 密集工作排到线程池；I/O 密集应使用真正的异步 API（`HttpClient`、`EF Core` 异步方法）。


**代码示例：**

```csharp
// Task：高层抽象，默认在线程池线程上运行，可返回结果、可取消
Task<int> task = Task.Run(() => 42);              // CPU 密集工作放线程池
int result = await task;                          // 异步等待结果

// Thread：底层执行单元，手动管理，每个线程独立 1MB 栈
var thread = new Thread(() => Console.WriteLine("子线程"));
thread.Start();
thread.Join();                                    // 阻塞等待结束（同步）

// 对比：I/O 密集应使用真正异步 API（HttpClient/EF Core 异步方法），
// 而不是开线程阻塞等待
```

### 3.3 什么是线程池（ThreadPool）？

- 线程池管理一组工作线程，任务提交后复用线程，避免频繁创建销毁线程的开销。
- 默认线程数：CPU 数 × 一些系数（随负载自适应）。
- **线程池饥饿（ThreadPool Starvation）**：线程被阻塞（如 sync-over-async、长阻塞 I/O、锁），任务排队等待，吞吐量骤降——表现为请求排队、超时。
- 最小线程数可配置：`ThreadPool.SetMinThreads`。


**代码示例：**

```csharp
// 线程池：复用线程，避免频繁创建/销毁的开销
// Task.Run / async 的 I/O 完成回调默认都在线程池线程上执行

ThreadPool.QueueUserWorkItem(_ => Console.WriteLine("池中线程执行"));

// 查看线程数
Console.WriteLine($"工作线程: {ThreadPool.ThreadCount}");

// 线程池饥饿：线程被阻塞（sync-over-async、长阻塞 I/O、死锁）时
// 新任务排队等待，表现为请求超时、吞吐骤降
// 应急可调高最小线程数（治标）：
// ThreadPool.SetMinThreads(64, 64);

// 根本解法：消除阻塞，全部改为真异步
```

### 3.4 `ConfigureAwait(false)` 是做什么的？什么时候用？（高频）

- 默认 `await` 会捕获当前 `SynchronizationContext`（UI 线程/请求上下文），完成后回到原上下文执行。
- `ConfigureAwait(false)`：**不捕获上下文**，继续在任意线程池线程执行，避免死锁、减少切换开销。
- **使用场景**：类库代码、后台服务（无 UI 上下文需求）；**不要**在 UI 代码用（需要回到 UI 线程更新界面）。
- **现代 ASP.NET Core 中无 SynchronizationContext**，死锁风险低，但仍建议库代码使用以减开销。


**代码示例：**

```csharp
// 默认 await 捕获当前 SynchronizationContext，完成后回到原上下文
// UI 场景：回到 UI 线程更新界面
await DoWorkAsync();
textBox.Text = "完成";                    // 需要回到 UI 上下文

// 类库/后台服务：不需要上下文，用 ConfigureAwait(false) 减少切换
public async Task<string> LoadAsync()
{
    var data = await File.ReadAllTextAsync("a.txt").ConfigureAwait(false);
    // 后续在任意线程池线程继续，避免死锁、降低开销
    return data;
}

// 注意：ASP.NET Core 无 SynchronizationContext，但类库代码仍建议加
// 不要在 UI 代码用（回不到 UI 线程）
```

### 3.5 什么是 sync-over-async 死锁？如何避免？（必问）

**要点：**
- 场景：UI 应用/旧 ASP.NET 中，同步上下文上调用 `.Result`/`.Wait()` 阻塞调用线程，而 await 的续延需要回到该上下文执行 → 互相等待 → 死锁。
- **避免**：
  1. async 一路向上，入口也异步；
  2. 需要桥接时：`Task.Run(() => AsyncMethod()).GetAwaiter().GetResult()`（脱离原上下文）；
  3. 类库内用 `ConfigureAwait(false)`。
- ASP.NET Core 中请求无同步上下文，但**阻塞线程池**同样降低吞吐，仍应避免。


**代码示例：**

```csharp
// 危险场景：同步上下文上调用 .Result/.Wait() 阻塞调用线程
// 而 await 续延需要回到该上下文执行 -> 互相等待 -> 死锁
public void Bad()
{
    // UI/旧 ASP.NET 中可能死锁：
    // var r = DoAsync().Result;      // 阻塞
}

public async Task<string> DoAsync()
{
    await Task.Delay(100);            // 续延想回到原上下文
    return "ok";
}

// 正确做法：
// 1. async 一路向上，入口也 async
public async Task<string> GoodAsync() => await DoAsync();

// 2. 必须桥接时：脱离原上下文执行
public string Bridge() =>
    Task.Run(() => DoAsync()).GetAwaiter().GetResult();

// 3. 类库内用 ConfigureAwait(false)，不捕获上下文
```

### 3.6 为什么 `async void` 很危险？

- `async void` 无法被 `await`，异常无法被调用方捕获，会直接抛到 `SynchronizationContext` 导致**进程崩溃**。
- 仅限 UI 事件处理器使用（WinForms/WPF 事件签名要求 void）。
- 其余场景一律 `async Task` / `async Task<T>`。


**代码示例：**

```csharp
// async void：异常无法被 await 捕获，直接抛到 SynchronizationContext
// 严重时导致进程崩溃
async void BadHandler()
{
    throw new InvalidOperationException("炸了");
}
try
{
    BadHandler();          // 这里 catch 不到！异常逃逸到上下文
}
catch (Exception)
{
    // 永远不会执行到这里
}

// 唯一合法场景：事件处理器（WinForms/WPF 事件签名要求 void）
button.Click += async (s, e) =>
{
    await Task.Delay(100);
};

// 其余场景一律 async Task / async Task<T>，异常可被 await 捕获
```

### 3.7 `ValueTask<T>` 与 `Task<T>` 的区别？

- `Task<T>`：引用类型，结果可复用、多次 await；有堆分配。
- `ValueTask<T>`：值类型，避免分配；**只能 await 一次**（不可多次 await/缓存），适合结果往往同步可得的场景（如缓存命中、短操作）。
- 用法：`ValueTask<T>` 用于低频异步、结果通常立即可得的方法签名。


**代码示例：**

```csharp
// Task<T>：引用类型，结果可多次 await、可缓存；有堆分配
public async Task<int> GetFromDbAsync() => await Task.FromResult(42);

// ValueTask<T>：值类型，避免分配；只能 await 一次
// 适合结果往往同步可得的场景（如缓存命中）
public ValueTask<int> GetAsync(int key)
{
    if (cache.TryGetValue(key, out int v))
        return new ValueTask<int>(v);      // 同步返回，零分配

    return new ValueTask<int>(LoadSlowAsync(key));  // 异步路径
}
static Dictionary<int, int> cache = new();
async Task<int> LoadSlowAsync(int key) { await Task.Delay(1); return key; }

// 注意：ValueTask 不可多次 await、不可缓存，一次消费
```

### 3.8 多线程与异步编程的区别？（高频）

**要点：**
- **多线程**：真正并行执行多个代码路径（每个线程独立），适合 CPU 密集；资源消耗大，需处理竞争/死锁。
- **异步**：单个线程可"挂起-继续"多个操作，适合 I/O 密集；不创建新线程，靠线程复用提高吞吐。
- 一句话：异步解决**等待**问题（不阻塞），多线程解决**并行计算**问题。
- 现代 .NET：I/O 用 async/await，CPU 并行用 `Parallel`/`PLINQ`/`Task.Run`。


**代码示例：**

```csharp
// 多线程：真正并行执行，适合 CPU 密集（利用多核）
Parallel.For(0, 10, i => Console.WriteLine($"线程 {Environment.CurrentManagedThreadId} 处理 {i}"));

// 异步：单线程可"挂起-继续"多个 I/O 操作，不创建线程，靠线程复用
static async Task DownloadAllAsync()
{
    var client = new HttpClient();
    // 同时发起多个 I/O，等待期间无线程被阻塞
    var tasks = Enumerable.Range(0, 5).Select(i => client.GetStringAsync($"https://x.com/{i}"));
    var results = await Task.WhenAll(tasks);
}

// 一句话：异步解决"等待"问题（不阻塞），多线程解决"并行计算"问题
// 现代 .NET：I/O 用 async/await，CPU 并行用 Parallel/PLINQ/Task.Run
```

### 3.9 什么是线程安全？`lock` 关键字如何工作？

- 线程安全：多个线程并发访问共享状态时结果正确。
- `lock (obj)` 是 `Monitor.Enter/Exit` 的语法糖，互斥访问临界区；对象引用作锁（不要锁值类型/字符串字面量）。
- `Monitor` 支持 `TryEnter` 超时；`lock` 不能跨 `await`（C# 编译报错）——异步互斥要用 `SemaphoreSlim`。
- 替代：`Interlocked`（原子操作）、`ReaderWriterLockSlim`（读写锁）、`SemaphoreSlim`、`Mutex`、`Concurrent*` 集合。


**代码示例：**

```csharp
private readonly object _lock = new();
private int _count;

public void Increment()
{
    lock (_lock)        // Monitor.Enter/Exit 语法糖，互斥进入临界区
    {
        _count++;
    }
}

// 注意：
// 1. 不要锁值类型/字符串字面量（可能被共享）
// 2. lock 不能跨 await（编译错误），异步互斥用 SemaphoreSlim
private readonly SemaphoreSlim _sem = new(1, 1);
public async Task UpdateAsync()
{
    await _sem.WaitAsync();
    try { await Task.Delay(10); } finally { _sem.Release(); }
}
```

### 3.10 什么是死锁（Deadlock）？如何避免？

- 死锁：两个以上线程各自持有锁等待对方释放 → 永久阻塞。
- **必要条件**：互斥、持有并等待、不可剥夺、循环等待。
- **避免**：锁顺序一致、减少锁持有时间、用超时（`TryEnter`）、避免嵌套锁、用无锁结构/异步。


**代码示例：**

```csharp
// 死锁：两个线程各自持有对方需要的锁，互相等待
object lockA = new(), lockB = new();

// 线程1：先 A 后 B
Task.Run(() => { lock (lockA) { Thread.Sleep(10); lock (lockB) { } } });
// 线程2：先 B 后 A  -> 与线程1 交叉等待，死锁
Task.Run(() => { lock (lockB) { Thread.Sleep(10); lock (lockA) { } } });

// 避免：
// 1. 锁顺序一致（都先 A 后 B）
// 2. 用超时 TryEnter，不无限等待
if (Monitor.TryEnter(lockA, TimeSpan.FromSeconds(1)))
{
    try { /* 临界区 */ } finally { Monitor.Exit(lockA); }
}
// 3. 减少锁持有时间；避免嵌套锁；考虑无锁结构/异步
```

### 3.11 什么是竞争条件（Race Condition）？

- 多线程并发访问共享数据，结果依赖执行时序而不可预测（如 `count++` 非原子）。
- 解决：`lock`、`Interlocked.Increment`、`ConcurrentDictionary`、不可变数据、同步集合。


**代码示例：**

```csharp
int count = 0;

// 竞争：count++ 不是原子操作（读-改-写），多线程交错执行结果不可预测
Parallel.For(0, 10000, _ => count++);   // 结果通常小于 10000！

// 解决1：Interlocked 原子操作
int safe = 0;
Parallel.For(0, 10000, _ => Interlocked.Increment(ref safe));  // == 10000

// 解决2：lock
// 解决3：并发集合 ConcurrentDictionary 等
// 解决4：不可变数据（多线程只读共享）
```

### 3.12 `volatile` 关键字的作用？

- 告诉编译器/运行时该字段每次读写都直接访问内存，禁止缓存优化，保证**可见性**（多线程读取最新值）。
- **不能保证原子性**：`volatile int` 的 `++` 仍非原子。
- 仅适用于基元类型（引用类型、int、bool 等）；复杂场景用 `Interlocked`/`lock`。


**代码示例：**

```csharp
// volatile：禁止编译器和 CPU 缓存该字段，保证多线程可见性
private volatile bool _running = true;

public void Stop() => _running = false;    // 其他线程能立刻看到

public void Work()
{
    while (_running)      // 没有 volatile 时可能永远读不到最新值
    {
        // 处理中
    }
}

// 注意：volatile 不保证原子性！volatile int 的 ++ 仍非原子
// 需要原子性用 Interlocked / lock
```

### 3.13 `Interlocked` 类提供什么能力？

- 原子操作：`Increment`、`Decrement`、`Add`、`Exchange`、`CompareExchange`。
- 无锁原子更新，性能高于 lock；典型：计数器、状态标志、`CompareExchange` 实现自旋锁。


**代码示例：**

```csharp
int counter = 0;

// 原子操作，无锁，性能高于 lock
Interlocked.Increment(ref counter);             // ++
Interlocked.Decrement(ref counter);             // --
Interlocked.Add(ref counter, 10);               // += 10
int old = Interlocked.Exchange(ref counter, 5); // 赋值并返回旧值

// CompareExchange：比较并交换（典型：CAS 实现无锁更新）
int value = 100;
int original = Interlocked.CompareExchange(ref value, 200, 100);
// 若 value==100 则设为 200；返回原值 100

// 经典用途：计数器、状态标志、自旋锁实现
```

### 3.14 什么是 `SemaphoreSlim`、`Mutex`、`ReaderWriterLockSlim`？

- `SemaphoreSlim`：信号量，限制同时访问的并发数；支持异步 `WaitAsync()`（**async 场景推荐**）。
- `Mutex`：跨进程互斥锁（系统级命名）；进程内优先用 `lock`。
- `ReaderWriterLockSlim`：多读单写；读多写少场景性能好。


**代码示例：**

```csharp
// SemaphoreSlim：限制同时访问的并发数，支持异步等待（async 场景推荐）
var sem = new SemaphoreSlim(3);          // 最多 3 个并发
await sem.WaitAsync();
try { await Task.Delay(50); } finally { sem.Release(); }

// Mutex：跨进程互斥（系统级命名）；进程内优先用 lock
using var mutex = new Mutex(false, @"Global\MyApp");
if (mutex.WaitOne(TimeSpan.Zero))
{
    // 单实例应用
    mutex.ReleaseMutex();
}

// ReaderWriterLockSlim：多读单写，读多写少场景性能好
var rw = new ReaderWriterLockSlim();
rw.EnterReadLock();  // 多个读可并发
try { /* 读 */ } finally { rw.ExitReadLock(); }
rw.EnterWriteLock(); // 写独占
try { /* 写 */ } finally { rw.ExitWriteLock(); }
```

### 3.15 并发集合有哪些？适用场景？

- `ConcurrentDictionary<TKey,T>`：并发字典（读多写多）。
- `ConcurrentQueue<T>`：线程安全 FIFO（生产者消费者）。
- `ConcurrentBag<T>`：无序集合（每个线程本地缓存）。
- `BlockingCollection<T>`：阻塞队列，支持 `CompleteAdding`、边界。
- `ConcurrentStack<T>`：LIFO。
- **注意**：普通 `Dictionary` + `lock` 在低并发下可能更简单；`ConcurrentDictionary` 的 `Count` 是近似值。


**代码示例：**

```csharp
// ConcurrentDictionary：并发读写字典（读多写多）
var dict = new ConcurrentDictionary<string, int>();
dict.TryAdd("a", 1);
dict.AddOrUpdate("a", 2, (_, old) => old + 1);   // 原子累加

// ConcurrentQueue：线程安全 FIFO（生产者-消费者）
var queue = new ConcurrentQueue<int>();
queue.Enqueue(1);
queue.TryDequeue(out int item);

// ConcurrentBag：无序集合，每线程本地缓存（分治归并）
var bag = new ConcurrentBag<int>();
bag.Add(1);

// BlockingCollection：阻塞队列，支持 CompleteAdding、有界
using var bc = new BlockingCollection<int>(boundedCapacity: 100);
bc.Add(1);
bc.CompleteAdding();          // 通知不再生产
// 消费者：foreach (var i in bc.GetConsumingEnumerable())

// 注意：ConcurrentDictionary.Count 是近似值；低并发下普通集合+lock 更简单
```

### 3.16 什么是 `Parallel`、`PLINQ`？

- `Parallel.For` / `Parallel.ForEach`：CPU 密集任务的并行执行（自动分块到多线程）。
- PLINQ：`.AsParallel().Where(...)` 并行 LINQ。
- 注意：I/O 密集不适合（应用异步）；并行有调度开销，任务太小反而更慢；结果合并需线程安全（`ConcurrentBag`、锁、`Aggregate`）。


**代码示例：**

```csharp
// Parallel：CPU 密集任务并行执行，自动分块到多线程
Parallel.For(0, 100, i => Compute(i));           // For
Parallel.ForEach(items, item => Process(item));  // ForEach

// PLINQ：并行 LINQ
var result = Enumerable.Range(1, 1000)
    .AsParallel()
    .Where(n => n % 2 == 0)
    .Select(n => n * 2)
    .ToArray();           // 注意：并行 LINQ 结果顺序不定

// 陷阱：
// 1. I/O 密集不适合（应使用异步）
// 2. 任务太小并行反而慢（调度开销）
// 3. 结果合并需线程安全（ConcurrentBag / lock / Aggregate）
static void Compute(int i) => Thread.SpinWait(100);
static void Process(int i) { }
```

### 3.17 什么是 `CancellationToken`？

- 协作式取消：`CancellationTokenSource` 发出取消信号，任务内检查 `token.IsCancellationRequested` 或调用 `ThrowIfCancellationRequested()`。
- 贯穿异步 API（`HttpClient`、EF Core、`Task.Delay`）；支持超时取消（`CancelAfter`）、链式取消（`CreateLinkedTokenSource`）。
- 取消是协作式：需要代码主动响应，不能强杀线程。


**代码示例：**

```csharp
// 协作式取消：不强杀线程，代码主动检查 token 响应
static async Task DownloadAsync(CancellationToken token)
{
    for (int i = 0; i < 100; i++)
    {
        token.ThrowIfCancellationRequested();   // 取消时抛 OperationCanceledException
        // token.IsCancellationRequested 也可手动检查

        await Task.Delay(10, token);            // 内置支持取消
    }
}

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)); // 超时自动取消
// cts.Cancel();                               // 手动取消
// 链式取消：CancellationTokenSource.CreateLinkedTokenSource(t1, t2);

try { await DownloadAsync(cts.Token); }
catch (OperationCanceledException) { Console.WriteLine("已取消"); }
```

### 3.18 什么是 `TaskCompletionSource<T>`？

- 手动控制 `Task<T>` 的完成/失败/取消：`SetResult`、`SetException`、`SetCanceled`。
- 用于把回调/事件包装成 Task（Task-based 异步模式），如 `Socket` 回调封装、等待某事件触发。


**代码示例：**

```csharp
// 手动控制 Task 的完成/失败/取消：把回调/事件包装成 Task
static Task<int> WaitForEventAsync()
{
    var tcs = new TaskCompletionSource<int>();

    var timer = new System.Timers.Timer(1000) { AutoReset = false };
    timer.Elapsed += (_, _) => tcs.SetResult(42);   // 事件触发 -> 完成 Task
    timer.Start();

    return tcs.Task;
}

int value = await WaitForEventAsync();   // 1 秒后得到 42
// 还有 SetException / SetCanceled 对应失败与取消
// 典型场景：Socket 回调、等待外部事件、实现自旋等待
```

### 3.19 `Task.Delay` 与 `Thread.Sleep` 的区别？

- `Thread.Sleep`：**阻塞**当前线程指定毫秒（浪费线程池线程）。
- `Task.Delay`：异步等待，不阻塞线程（返回 Task，可 await）；支持 `CancellationToken`。
- 服务端代码一律用 `Task.Delay`。


**代码示例：**

```csharp
// Thread.Sleep：阻塞当前线程（浪费线程池线程，服务端绝对避免）
Thread.Sleep(1000);

// Task.Delay：异步等待，不阻塞线程（返回 Task，可 await）
await Task.Delay(1000);          // 线程被释放回线程池

// Task.Delay 支持取消
using var cts = new CancellationTokenSource(500);
try { await Task.Delay(5000, cts.Token); }
catch (OperationCanceledException) { Console.WriteLine("被取消"); }

// 服务端代码一律用 Task.Delay
```

### 3.20 什么是生产者-消费者模式？在 .NET 中如何实现？

- 生产者生产数据、消费者处理数据，中间用缓冲队列解耦。
- 实现：`BlockingCollection<T>`、`Channel<T>`（推荐，异步友好，.NET Core 3+）、`ConcurrentQueue` + 信号量。
- 场景：消息处理、任务调度、日志批处理。


**代码示例：**

```csharp
// 生产者-消费者：中间用缓冲队列解耦，生产/消费速度可不同
var queue = new Channel<int>(new BoundedChannelOptions(10)
{
    FullMode = BoundedChannelFullMode.Wait   // 有界 + 背压
});

// 生产者
_ = Task.Run(async () =>
{
    for (int i = 0; i < 100; i++)
        await queue.Writer.WriteAsync(i);
    queue.Writer.Complete();                 // 通知消费完成
});

// 消费者
await foreach (var item in queue.Reader.ReadAllAsync())
{
    Console.WriteLine($"消费 {item}");
}

// 其他实现：BlockingCollection<T>（同步）、ConcurrentQueue + SemaphoreSlim
// 场景：消息处理、任务调度、日志批处理
```

### 3.21 什么是 `Channel<T>`？

- 高性能异步管道：`Channel.CreateUnbounded/Bounded<T>()`，`WriteAsync`/`ReadAllAsync`。
- 支持背压（Bounded）、取消、完成；是现代 .NET 异步生产者-消费者的首选。


**代码示例：**

```csharp
// 高性能异步管道，支持背压、取消、完成
var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(100)
{
    FullMode = BoundedChannelFullMode.Wait,   // 满时让生产者等待（背压）
    SingleReader = true,
    SingleWriter = false
});

// 写端
await channel.Writer.WriteAsync(item);        // 满时异步等待
channel.Writer.TryWrite(item);                // 非阻塞尝试

// 读端
await foreach (var item in channel.Reader.ReadAllAsync(ct))
{
    Process(item);
}

// 完成/错误传播
channel.Writer.Complete(exception: null);

// 现代 .NET 异步生产者-消费者首选（.NET Core 3+）
static void Process(int i) { }
```

### 3.22 什么是数据竞争与内存可见性？

- 数据竞争：多个线程无同步地读写共享数据。
- 内存可见性：无同步时，一个线程的写可能对其他线程不可见（CPU 缓存/指令重排）；`lock`、`volatile`、`Interlocked` 建立内存屏障保证可见性。


**代码示例：**

```csharp
// 数据竞争：多线程无同步地读写共享数据，结果不确定
int shared = 0;
Parallel.For(0, 10000, _ => shared++);    // 数据竞争，结果不可预测

// 内存可见性：无同步时一个线程的写可能对其他线程不可见
// （CPU 缓存 / 指令重排导致），需要内存屏障
// lock / volatile / Interlocked 内部都会建立内存屏障保证可见性

// 正确做法：使用同步原语
int safe = 0;
Parallel.For(0, 10000, _ => Interlocked.Increment(ref safe));
```

### 3.23 什么是 `ThreadStatic` / `AsyncLocal<T>`？

- `[ThreadStatic]`：每个线程独立的静态字段副本。
- `AsyncLocal<T>`：**异步流**中传播的值（不随线程，随执行流），适合传递调用上下文（如 TraceId、租户信息）。
- 现代代码优先 `AsyncLocal`。


**代码示例：**

```csharp
// [ThreadStatic]：每个线程独立的静态字段副本（不随异步流传播）
[ThreadStatic] private static int _perThread;

// AsyncLocal<T>：随"执行流"传播的值（async/await 中不丢失），
// 适合传递调用上下文：TraceId、租户信息、用户标识
private static readonly AsyncLocal<string> _traceId = new();

public static string TraceId
{
    get => _traceId.Value ?? "";
    set => _traceId.Value = value;
}

static async Task OuterAsync()
{
    TraceId = Guid.NewGuid().ToString("N");
    await InnerAsync();          // InnerAsync 内 TraceId 仍然可见
}

static Task InnerAsync()
{
    Console.WriteLine(TraceId);  // 与外部一致（而 ThreadStatic 会丢失）
    return Task.CompletedTask;
}

// 现代代码优先 AsyncLocal
```

### 3.24 什么是 `IAsyncEnumerable<T>`？（C# 8+）

- 异步迭代器：`async` + `yield return`，配合 `await foreach` 消费。
- 场景：分页查询、流式处理、消息流（逐个异步产出元素，避免一次加载全部）。


**代码示例：**

```csharp
// 异步迭代器：async + yield return，逐个异步产出元素
static async IAsyncEnumerable<int> GetPagesAsync()
{
    for (int i = 0; i < 5; i++)
    {
        await Task.Delay(10);          // 每页异步加载
        yield return i;
    }
}

// 消费：await foreach（见 3.26）
await foreach (var page in GetPagesAsync())
{
    Console.WriteLine(page);
}

// 场景：分页查询、流式处理、消息流——避免一次加载全部
// EF Core 也支持：await foreach (var o in context.Orders.AsAsyncEnumerable())
```

### 3.25 什么是 `Task.WhenAll` / `Task.WhenAny` / `Task.WaitAll`？

- `WhenAll`：等待所有 Task 完成（异步，推荐），返回聚合结果；有异常聚合抛。
- `WhenAny`：任一完成即返回；用于超时竞争、首个响应。
- `WaitAll`：**阻塞**等待全部完成（避免在 async 上下文使用）。
- 注意：`WhenAll` 中某个 Task 失败会抛 `AggregateException`；可用 `WhenAll(ops.Select(o => o.ContinueWith(...)))` 容错。


**代码示例：**

```csharp
// WhenAll：等待所有 Task 完成（异步，推荐），返回聚合结果
var tasks = Enumerable.Range(1, 5).Select(i => GetAsync(i));
int[] results = await Task.WhenAll(tasks);

// WhenAny：任一完成即返回；用于超时竞争、首个响应
var winner = await Task.WhenAny(GetAsync(1), GetAsync(2));
Console.WriteLine(await winner);

// 超时竞争模式
var delay = Task.Delay(2000);
var work = GetAsync(1);
var first = await Task.WhenAny(work, delay);
if (first == delay) Console.WriteLine("超时");

// WaitAll：阻塞等待（避免在 async 上下文使用）
Task.WaitAll(tasks.ToArray());

// 容错：WhenAll 中某个失败会抛 AggregateException
// 可先包一层 ContinueWith 收集所有结果再判断
static async Task<int> GetAsync(int i) { await Task.Delay(10); return i; }
```

### 3.26 什么是异步流？`await foreach`？

- 消费 `IAsyncEnumerable<T>`；每迭代一次异步获取一个元素。
- 例：`await foreach (var item in GetDataAsync()) { ... }`。


**代码示例：**

```csharp
static async IAsyncEnumerable<string> ReadLinesAsync(string path)
{
    using var reader = new StreamReader(path);
    while (await reader.ReadLineAsync() is { } line)
    {
        yield return line;              // 逐个异步产出
    }
}

// await foreach：每迭代一次异步获取一个元素
await foreach (var line in ReadLinesAsync("log.txt"))
{
    Console.WriteLine(line);
}

// 支持取消
var cts = new CancellationTokenSource();
await foreach (var line in ReadLinesAsync("log.txt").WithCancellation(cts.Token))
{
    Process(line);
}
static void Process(string s) { }
```

### 3.27 什么是"线程局部存储"？

- 每个线程独立存储空间（`ThreadLocal<T>`、`[ThreadStatic]`），避免共享冲突；`ThreadLocal<T>` 支持初始化和值工厂，且可释放。


**代码示例：**

```csharp
// ThreadLocal<T>：每个线程独立存储，支持初始化/值工厂，且可释放
var local = new ThreadLocal<int>(() => 42);   // 每线程初始值 42

Parallel.For(0, 10, i =>
{
    local.Value = i * 10;      // 每个线程各自维护自己的值，无竞争
    Console.WriteLine(local.Value);
});
// 最终释放
local.Dispose();

// 对比：
// [ThreadStatic]：静态字段级线程局部（见 3.23）
// AsyncLocal<T>：随执行流传播（async/await 中推荐）

// 用途：避免共享冲突的每线程缓存、每线程随机种子等
```

### 3.28 什么是锁的粒度与性能权衡？

- 锁粒度越小并发越高，但获取锁次数增加；大锁降低并发但简单。
- 权衡点：临界区长度、竞争频率、锁内是否做 I/O（**绝不在锁内做 I/O**）、无锁替代方案。

---

<a id="c4"></a>

## 四、.NET 平台与 C# 新特性

### 4.1 .NET Framework、.NET Core、.NET 5+ 有什么区别？（必问）

**要点：**
- **.NET Framework**：Windows 专属、随系统/独立安装、闭源（主要部分开源）、性能与跨平台受限，已停止新特性（4.8 为最终版，仍受支持维护）。
- **.NET Core**：跨平台（Windows/Linux/macOS）、开源、模块化、高性能，1.0 → 3.1。
- **.NET 5+**：.NET Core 的延续，统一为 **.NET**（5/6/7/8/9/10），一个平台覆盖 Web、桌面（WinForms/WPF/MAUI）、云、移动、IoT；LTS 版本：6、8、10。
- 现代新项目用 .NET 8+；`dotnet --list-sdks` 查看版本。
- 兼容性：.NET 5+ 无法直接运行 .NET Framework 程序集（二进制不兼容），需迁移。

### 4.2 .NET 的 LTS 与 STS 版本策略？

- LTS（Long Term Support，长期支持）：偶数版本（6、8、10），支持约 3 年。
- STS（Standard Term Support，标准支持）：奇数版本（7、9），支持约 18 个月。
- 生产环境推荐 LTS。

### 4.3 C# 各版本主要新特性（C# 9 → C# 13）？（高频）

| 版本 | 关键特性 |
|---|---|
| C# 9 (.NET 5) | `record`、`init`、`with`、顶层语句、模式匹配增强 |
| C# 10 (.NET 6) | `record struct`、全局 using、文件级命名空间、`const` 插值字符串 |
| C# 11 (.NET 7) | 原始字符串（`"""`）、`required` 成员、泛型数学、UTF-8 字符串字面量 |
| C# 12 (.NET 8) | 主构造函数（class）、集合表达式 `[...]`、`ref readonly`、`params` 集合 |
| C# 13 (.NET 9) | 主构造函数增强（隐式字段）、`ref` 局部变量进迭代器/async、`allows ref struct` 泛型约束 |

**加分点**：常被追问——顶层语句如何编译（Main 自动生成）；`record` 值相等性的实现；`required` 与构造函数关系。


**代码示例：**

```csharp
// C# 9：record / init / with / 顶层语句
public record Person(string Name, int Age);           // record：值相等性
var p = new Person("张三", 18) { /* init 属性 */ };
var p2 = p with { Age = 20 };                         // with：非破坏性修改

// C# 10：record struct / 全局 using / 文件级命名空间 / const 插值
public readonly record struct Point(int X, int Y);
const string Host = "api";
const string Url = $"https://{Host}/v1";              // const 插值字符串

// C# 11：原始字符串 / required / 泛型数学
string sql = """
    SELECT * FROM Users
    WHERE Id = @id
    """;                                              // 原始字符串：无需转义
public class Order { public required long Id { get; set; } }  // 必填成员

// C# 12：主构造函数 / 集合表达式
public class CtorDemo(string name) { public string Name => name; }
int[] nums = [1, 2, 3];                               // 集合表达式
List<int> more = [.. nums, 4, 5];                     // 展开运算符

// C# 13：主构造函数隐式字段 / ref 进 async / allows ref struct
```

### 4.4 什么是顶层语句（Top-level Statements）？

- C# 9+：`Program.cs` 中直接写代码，不用显式 `Main`/`class`，编译器自动生成入口点。
- 适合小型程序、脚本、`Program.cs` 简洁化；大型项目仍可用传统结构。


**代码示例：**

```csharp
// Program.cs（C# 9+）：直接写代码，编译器自动生成 Main 入口
using System;

Console.WriteLine("Hello 顶层语句!");
int sum = Enumerable.Range(1, 100).Sum();
Console.WriteLine(sum);

// 等价于传统写法：
// class Program
// {
//     static void Main(string[] args)
//     {
//         Console.WriteLine("Hello 顶层语句!");
//     }
// }
// 适合小型程序、脚本、Program.cs 简洁化；大型项目仍可用传统结构
```

### 4.5 什么是 `System.Text.Json` 与 Newtonsoft.Json 的区别？

- `System.Text.Json`：官方内置、高性能、低分配；默认严格（大小写敏感）、特性支持有限。
- Newtonsoft.Json：老牌、功能全（`JsonConvert`、动态 `JObject`、条件序列化、循环引用处理），性能较慢。
- 选择：新项目默认 `System.Text.Json`；复杂序列化需求用 Newtonsoft。
- 性能优化：源生成（`JsonSerializerContext`）避免反射。


**代码示例：**

```csharp
public class Order
{
    public int Id { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("customer")]
    public string Customer { get; set; } = "";
}

// System.Text.Json：官方内置、高性能、低分配，默认大小写敏感
var order = new Order { Id = 1, Customer = "张三" };
string json = System.Text.Json.JsonSerializer.Serialize(order);
// {"Id":1,"customer":"张三"}

// 高性能：源生成（避免反射）
[System.Text.Json.Serialization.JsonSerializable(typeof(Order))]
partial class AppJsonContext : System.Text.Json.Serialization.JsonSerializerContext { }

// Newtonsoft.Json：功能全（JObject、条件序列化、循环引用），性能较慢
// string json2 = JsonConvert.SerializeObject(order);
// 新项目默认 System.Text.Json；复杂序列化需求用 Newtonsoft
```

### 4.6 什么是 `Minimal API`？

- .NET 6+：`app.MapGet("/", () => "Hello")` 极简定义端点，不建 Controller。
- 适合小服务、微服务、函数、原型；性能略优（无 MVC 层开销）。
- 复杂项目仍用 Controller + MVC。


**代码示例：**

```csharp
// Program.cs（.NET 6+）：极简定义端点，不建 Controller
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/users/{id:int}", (int id) => new { Id = id, Name = "张三" });
app.MapPost("/users", (UserDto dto) => Results.Created($"/users/{dto.Id}", dto));
app.MapDelete("/users/{id:int}", (int id) => Results.NoContent());

app.Run();

record UserDto(int Id, string Name);

// 适合小服务、微服务、函数、原型；性能略优（无 MVC 层开销）
// 复杂项目仍用 Controller + MVC
```

### 4.7 什么是 `WebApplication` 与 `WebApplicationBuilder`？

- `WebApplicationBuilder`：配置宿主（服务注册、配置、日志），`builder.Build()` 生成 `WebApplication`。
- `WebApplication`：既是应用又是中间件管道，`app.MapGet`、`app.Use...`、`app.Run()`。
- 这是 .NET 6+ 默认的最小宿主模型。


**代码示例：**

```csharp
// WebApplicationBuilder：配置宿主（服务注册、配置、日志）
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();              // 注册服务到 DI
builder.Services.AddEndpointsApiExplorer();

// WebApplication：既是应用又是中间件管道
var app = builder.Build();

app.UseHttpsRedirection();                       // 配置中间件管道
app.MapControllers();                            // 映射端点
app.Run();                                       // 启动

// .NET 6+ 默认的最小宿主模型：builder 负责组装，app 负责运行
```

### 4.8 什么是 `IHost` / `IHostedService` / `BackgroundService`？

- `IHost`：通用主机，承载后台服务与应用生命周期。
- `IHostedService`：接口含 `StartAsync`/`StopAsync`，宿主启动/停止时执行。
- `BackgroundService`：`ExecuteAsync` 抽象基类，长期运行的后台任务（队列消费、定时任务、消息监听）。
- 注册：`builder.Services.AddHostedService<T>()`。


**代码示例：**

```csharp
// BackgroundService：长期运行的后台任务（队列消费、定时任务、消息监听）
public class CleanupWorker : BackgroundService
{
    private readonly ILogger<CleanupWorker> _logger;

    public CleanupWorker(ILogger<CleanupWorker> logger) => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)   // 直到应用停止
        {
            _logger.LogInformation("执行清理...");
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // 定时循环
        }
    }
}

// 注册
// builder.Services.AddHostedService<CleanupWorker>();

// IHostedService 是接口（StartAsync/StopAsync），BackgroundService 是其抽象基类
// IHost：通用主机，承载后台服务与应用生命周期
```

### 4.9 什么是 `Options` 模式（`IOptions<T>`）？

- 强类型配置：`services.Configure<T>(Configuration.GetSection("X"))`。
- `IOptions<T>`：应用启动时读取一次（不可重载）；`IOptionsSnapshot<T>`：每次请求读取（支持配置重载）；`IOptionsMonitor<T>`：监听变化。
- 验证：`[Required]`/`[Range]` 数据注解 + `ValidateOnStart`。


**代码示例：**

```csharp
// 1. 定义强类型配置类
public class RedisOptions
{
    public string ConnectionString { get; set; } = "";
    public int Database { get; set; }
}

// 2. 绑定配置
builder.Services.Configure<RedisOptions>(
    builder.Configuration.GetSection("Redis"));

// 3. 注入使用
public class CacheService
{
    private readonly RedisOptions _opt;
    public CacheService(IOptions<RedisOptions> opt) => _opt = opt.Value;
    // IOptions<T>：启动时读取一次（不可重载）
    // IOptionsSnapshot<T>：每次请求读取（支持配置重载）
    // IOptionsMonitor<T>：监听变更（IOptionsMonitor<T>.CurrentValue + OnChange）
}

// appsettings.json:
// { "Redis": { "ConnectionString": "localhost:6379", "Database": 0 } }
```

### 4.10 什么是配置系统（Configuration）？

- 分层配置源：appsettings.json、环境变量、命令行、用户机密、Azure Key Vault 等，按**注册顺序后者覆盖前者**。
- `IConfiguration` 按 `Section:Key` 访问；`GetSection`、`GetValue<T>`、`Bind`。
- 环境区分：`ASPNETCORE_ENVIRONMENT`（Development/Production）。


**代码示例：**

```csharp
// 分层配置源（后者覆盖前者）：
// appsettings.json -> appsettings.{Environment}.json -> 环境变量 -> 命令行
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// 按 Section:Key 访问
string conn = config.GetConnectionString("Default")!;      // "Server=.;Database=db"
string mode = config["FeatureFlags:NewOrder"]!;            // 冒号分隔层级

// 强类型读取
var redis = config.GetSection("Redis").Get<RedisOptions>();

// GetValue<T>：带默认值
int timeout = config.GetValue<int>("TimeoutSeconds", 30);

// 环境区分：ASPNETCORE_ENVIRONMENT=Development/Production
// 决定加载哪个 appsettings.{env}.json
class RedisOptions { public string ConnectionString { get; set; } = ""; }
```

### 4.11 什么是依赖注入（DI）？什么是 IoC？（必问）

**要点：**
- **IoC（控制反转）**：对象创建和依赖关系的控制权从使用方反转给容器。
- **DI（依赖注入）**：IoC 的实现方式——通过构造函数/属性/方法把依赖传入，而不是类内部 `new`。
- **好处**：降低耦合、便于单元测试（mock 替换）、便于更换实现、生命周期统一管理。
- **反模式**：`ServiceLocator`（到处从容器取服务）、构造函数依赖过多（>4 个考虑重构）、静态服务。


**代码示例：**

```csharp
// 不依赖注入（反例）：类内部 new，紧耦合，难以测试
public class OrderService
{
    private readonly SqlOrderRepo _repo = new();   // 硬编码依赖
}

// 依赖注入：通过构造函数把依赖传入（IoC 容器管理创建）
public interface IOrderRepository { Order? GetById(long id); }
public class SqlOrderRepository : IOrderRepository
{
    public Order? GetById(long id) => null;
}

public class OrderService(          // 主构造函数 + DI
    IOrderRepository repo)
{
    public Order? Get(long id) => repo.GetById(id);
}

// 注册到容器
// builder.Services.AddScoped<IOrderRepository, SqlOrderRepository>();
// builder.Services.AddScoped<OrderService>();

// 好处：降低耦合、便于单测（Mock 替换）、更换实现不改业务代码
class Order { }
```

### 4.12 什么是 `NuGet`？

- .NET 包管理器：`Install-Package` / `dotnet add package`；包源（nuget.org、私有源）。
- 解决依赖传递、版本冲突（`NU1605` 等）；`PackageReference` 声明在 .csproj。

### 4.13 什么是 `csproj` 的 SDK 风格项目？

- 简洁的项目文件：`<Project Sdk="Microsoft.NET.Sdk">`，默认包含所有 .cs 文件。
- 常用属性：`TargetFramework`、`Nullable`、`ImplicitUsings`、`LangVersion`。


**代码示例：**

```xml
<!-- 简洁的项目文件：默认包含所有 .cs 文件 -->
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>          <!-- 可空引用类型检查 -->
    <ImplicitUsings>enable</ImplicitUsings>  <!-- 隐式 using -->
    <LangVersion>latest</LangVersion>    <!-- 语言版本 -->
    <InvariantGlobalization>true</InvariantGlobalization>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
  </ItemGroup>

</Project>
```

### 4.14 什么是 `global.json`？

- 指定项目使用的 SDK 版本：`dotnet new globaljson`；保证团队构建一致性。


**代码示例：**

```json
// global.json：指定项目使用的 SDK 版本，保证团队构建一致
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestMinor"   // 找不到精确版本时的策略
  }
}

// 生成：dotnet new globaljson --sdk-version 8.0.100
// 查看本机 SDK：dotnet --list-sdks
```

### 4.15 什么是 `dotnet` CLI 常用命令？

- `dotnet new`（模板）、`dotnet build`、`dotnet run`、`dotnet test`、`dotnet publish`（发布）、`dotnet restore`、`dotnet ef`（EF 工具）、`dotnet watch`（热重载）。


**代码示例：**

```bash
dotnet new webapi -n MyApi          # 创建 Web API 项目
dotnet new sln -n MyApp             # 创建解决方案
dotnet sln add MyApi                # 项目加入解决方案

dotnet restore                      # 还原依赖
dotnet build                        # 编译
dotnet run                          # 运行
dotnet test                         # 运行测试
dotnet watch run                    # 热重载运行
dotnet publish -c Release -o out    # 发布
dotnet ef migrations add Init       # EF 迁移（需 dotnet-ef 工具）
dotnet ef database update           # 应用迁移
dotnet add package Newtonsoft.Json  # 安装 NuGet 包
dotnet --list-sdks                  # 查看已装 SDK
```

---

<a id="c5"></a>

## 五、ASP.NET Core

### 5.1 ASP.NET Core 是什么？与 ASP.NET（Framework）的区别？（必问）

**要点：**
- ASP.NET Core 是开源、跨平台、高性能的 Web 框架，可用于 Web API、MVC、实时应用（SignalR）、gRPC。
- **与旧 ASP.NET 的区别**：
  1. 跨平台（Windows/Linux/macOS），可部署 IIS、Nginx、Docker；
  2. 性能更高（Kestrel、轻量管道）；
  3. 内置依赖注入；
  4. 模块化中间件管道；
  5. 云原生、容器化友好；
  6. 不依赖 `System.Web`，模块更轻量；
  7. 统一框架（MVC + Web API 合一）。

### 5.2 ASP.NET Core 的启动流程是什么？（必问）

**要点：**
1. 创建 `WebApplicationBuilder`（`Program.cs` 顶层语句）；
2. 加载配置（appsettings.json、环境变量等）；
3. 注册服务到 DI 容器（`builder.Services.Add...`）；
4. 构建 `app`；
5. 配置中间件管道（`app.Use...`）；
6. 配置路由/端点（`MapGet`、`MapControllers`）；
7. `app.Run()` 启动应用。

**一句话**：先注册服务，再配置请求处理管道，最后启动 Web 服务。


**代码示例：**

```csharp
// Program.cs 标准启动流程（.NET 6+）
var builder = WebApplication.CreateBuilder(args);      // 1. 创建宿主构建器

// 2. 加载配置：appsettings.json、环境变量等（builder.Configuration）
// 3. 注册服务到 DI 容器
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var app = builder.Build();                             // 4. 构建应用

// 5. 配置中间件管道（顺序重要）
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();                   // 开发环境异常页
}
app.UseHttpsRedirection();
app.UseAuthentication();                               // 认证
app.UseAuthorization();                                // 授权（必须在认证之后）

// 6. 配置路由/端点
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();                                             // 7. 启动
```

### 5.3 什么是中间件（Middleware）？（必问）

**要点：**
- 中间件是处理 HTTP 请求/响应的组件，组成**管道**：请求按注册顺序流入，响应按反方向返回。
- 每个中间件可：处理请求、调用下一个（`next()`）、处理响应、短路（不调 next）。
- 典型中间件：异常处理、认证、授权、静态文件、路由、日志、CORS、HTTPS 重定向。
- 顺序至关重要：`UseAuthentication` 必须在 `UseAuthorization` 之前；异常处理中间件尽量靠前；`UseRouting` 先于端点执行。


**代码示例：**

```csharp
// 自定义中间件：请求按注册顺序流入，响应按反方向返回（洋葱模型）
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("请求进入: {Path}", context.Request.Path);

        // 调用下一个中间件（也可不调用 -> 短路，直接返回）
        await _next(context);

        _logger.LogInformation("响应离开: {Code}", context.Response.StatusCode);
    }
}

// 注册
app.UseMiddleware<RequestLoggingMiddleware>();
// 或内联：app.Use(async (ctx, next) => { ...; await next(); ... });
```

### 5.4 `Use`、`Run`、`Map` 的区别？

- **`Use`**：注册可传递的中间件（调用 `next` 继续）；可短路。
- **`Run`**：终止管道，不调用 next（终点站）。
- **`Map`**：按路径分支（`/admin` 走另一管道）；`MapWhen` 按条件分支。
- 例：`app.Map("/health", h => h.Run(...))`。


**代码示例：**

```csharp
app.Use(async (context, next) =>       // Use：可传递，调用 next 继续（也可短路）
{
    Console.WriteLine("1 进入");
    await next();
    Console.WriteLine("1 离开");
});

app.Run(async context =>               // Run：终止管道，不调用 next（终点站）
{
    await context.Response.WriteAsync("Hello");
});

app.Map("/health", h =>                // Map：按路径分支
{
    h.Run(async ctx => await ctx.Response.WriteAsync("healthy"));
});

// MapWhen：按条件分支
app.MapWhen(ctx => ctx.Request.Query.ContainsKey("debug"),
    branch => branch.UseDeveloperExceptionPage());
```

### 5.5 什么是依赖注入（DI）？三种生命周期？（必问）

**要点：**
- DI：对象的创建与依赖交给容器管理，通过构造函数注入。
- **生命周期**：
  - `Transient`：**每次解析**都新建实例（轻量无状态服务）；
  - `Scoped`：**每个请求/Scope 内**一个实例（DbContext、UnitOfWork）；
  - `Singleton`：**整个应用**一个实例（无状态工具类、缓存、配置）。
- **生命周期倒挂（Captive Dependency）**：Singleton 依赖 Scoped/Transient 是危险的——Singleton 会长期持有请求级实例（如 DbContext），导致状态混乱、连接/变更跟踪器泄漏，甚至运行时报错（验证模式可发现）。反之（Scoped 注入 Singleton）安全。


**代码示例：**

```csharp
// Transient：每次解析都新建实例（轻量无状态服务）
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Scoped：每个请求/Scope 内一个实例（DbContext、UnitOfWork）
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Singleton：整个应用一个实例（无状态工具类、缓存、配置）
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// 生命周期倒挂（Captive Dependency）危险示例：
// Singleton 依赖 Scoped（如 DbContext）-> Singleton 长期持有请求级实例
// 导致 ChangeTracker 无限增长、连接泄漏，甚至运行时报错
// 验证模式可发现：
builder.Services.AddOptions<DbContextOptions<AppDbContext>>().ValidateOnStart();
```

### 5.6 `IConfiguration` 与 `IOptions<T>` 的区别？

- `IConfiguration`：直接按键访问配置，灵活但非强类型。
- `IOptions<T>`：绑定到强类型类（`GetSection("X").Get<T>()`），可维护、可验证；推荐业务配置使用。
- 家族：`IOptions<T>`（启动时快照）、`IOptionsSnapshot<T>`（每次请求重读，支持重载）、`IOptionsMonitor<T>`（监听变更）。

### 5.7 `[ApiController]` 特性有什么作用？（高频）

- 自动模型验证（验证失败自动返回 400）；
- 参数绑定来源自动推断（复杂类型默认 `[FromBody]`，简单类型从路由/Query 推断）；
- 自动处理 `BadRequest` 响应格式（`ValidationProblemDetails`）；
- 要求特性路由（`[Route]`），不兼容约定路由。
- 只读 API：`[HttpGet]` 自动推断 `[FromQuery]`/`[FromRoute]`。


**代码示例：**

```csharp
[ApiController]                       // 自动模型验证、绑定来源推断、返回 ValidationProblemDetails
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{id:int}")]             // 要求特性路由
    public ActionResult<UserDto> Get(int id)          // 简单类型自动从路由/Query 绑定
    {
        return Ok(new UserDto(id, "张三"));
    }

    [HttpPost]
    public ActionResult<UserDto> Create([FromBody] CreateUserRequest req)  // 复杂类型自动 FromBody
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);  // 自动验证失败时无需手写
        return CreatedAtAction(nameof(Get), new { id = 1 }, new UserDto(1, req.Name));
    }
}
record UserDto(int Id, string Name);
record CreateUserRequest(string Name);
```

### 5.8 什么是 Model Binding？有哪些来源？

- Model Binding：把 HTTP 请求数据自动绑定到控制器参数/模型属性。
- 来源：路由（`[FromRoute]`）、Query（`[FromQuery]`）、Body（`[FromBody]`）、Form（`[FromForm]`）、Header（`[FromHeader]`）、Services（`[FromServices]`）。


**代码示例：**

```csharp
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpGet("{id:long}")]                                  // 路由：{id}
    public IActionResult GetById([FromRoute] long id) { return Ok(id); }

    [HttpGet]
    public IActionResult Search([FromQuery] int page,       // Query：?page=1&size=20
                                [FromQuery] int size = 20) { return Ok(new { page, size }); }

    [HttpPost]
    public IActionResult Create([FromBody] OrderDto dto)    // Body：JSON
    { return Ok(dto); }

    [HttpPost("form")]
    public IActionResult Upload([FromForm] IFormFile file)  // Form：表单
    { return Ok(file.FileName); }

    [HttpGet("header")]
    public IActionResult ReadHeader([FromHeader] string token) // Header
    { return Ok(token); }

    // [FromServices]：从 DI 容器取服务
}
record OrderDto(string No);
```

### 5.9 什么是 Model Validation？

- 数据注解：`[Required]`、`[StringLength]`、`[Range]`、`[EmailAddress]`、`[RegularExpression]`、自定义 `ValidationAttribute`。
- `[ApiController]` 下自动验证并返回 400；非 API 控制器用 `ModelState.IsValid` 手动判断。
- 也可用 FluentValidation 做复杂校验。


**代码示例：**

```csharp
public class RegisterRequest
{
    [Required(ErrorMessage = "用户名必填")]                     // 必填
    [StringLength(20, MinimumLength = 3)]                       // 长度
    public string UserName { get; set; } = "";

    [EmailAddress]                                              // 邮箱格式
    public string Email { get; set; } = "";

    [Range(18, 65)]                                             // 范围
    public int Age { get; set; }

    [RegularExpression(@"^\d{11}$")]                            // 正则
    public string Phone { get; set; } = "";
}

// [ApiController] 下：验证失败自动返回 400 ValidationProblemDetails
// 非 API 控制器：
// if (!ModelState.IsValid) return BadRequest(ModelState);

// 复杂校验可用 FluentValidation（独立类，集中规则）
```

### 5.10 `IActionResult` 与 `ActionResult<T>` 的区别？

- `IActionResult`：返回多种结果类型（`Ok()`、`NotFound()`、`BadRequest()`、`File()` 等），灵活但无类型约束。
- `ActionResult<T>`：约束返回 `T` 或状态码结果，Swagger 文档更准确；Web API 推荐。
- 都是 `Task<...>` 异步包装的返回值。


**代码示例：**

```csharp
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    // IActionResult：灵活，可返回多种结果，但无类型约束
    [HttpGet("old/{id}")]
    public IActionResult GetOld(int id)
    {
        var order = Find(id);
        if (order is null) return NotFound();          // 404
        return Ok(order);                              // 200
    }

    // ActionResult<T>：约束返回 T 或状态码结果，Swagger 文档更准确（推荐）
    [HttpGet("{id}")]
    public ActionResult<Order> Get(int id)
    {
        var order = Find(id);
        if (order is null) return NotFound();          // 仍可返回状态码
        return order;                                  // 自动包装为 200
    }

    static Order? Find(int id) => id == 1 ? new Order() : null;
}
class Order { public int Id { get; set; } }
```

### 5.11 Filter 与 Middleware 的区别？（高频）

- **Middleware**：作用于**整个请求管道**，更底层通用——认证、异常、日志、静态文件等全局逻辑。
- **Filter**：作用于 **MVC/API 执行流程**（控制器与 Action 层面）——权限、参数校验、Action 前后处理、结果封装。
- 过滤器的类型（按执行顺序）：Authorization Filter → Resource Filter → Action Filter → Exception Filter → Result Filter。
- 一句话：Middleware 处理管道级，Filter 处理 MVC 级。

### 5.12 什么是路由？特性路由与约定路由？

- 路由：URL → 控制器/Action（端点）的映射。
- 特性路由：`[Route("api/[controller]")]`、`[HttpGet("{id}")]` 写在成员上；灵活、API 首选。
- 约定路由：`MapControllerRoute(name, pattern, defaults)` 集中配置；传统 MVC 风格。
- 路由优先级、参数约束（`{id:int}`）、默认值、可选参数。


**代码示例：**

```csharp
// 特性路由（API 首选）：写在成员上
[ApiController]
[Route("api/[controller]")]                    // /api/products
public class ProductsController : ControllerBase
{
    [HttpGet]                                   // GET /api/products
    public IActionResult List() => Ok();

    [HttpGet("{id:int}")]                       // GET /api/products/5（:int 约束）
    public IActionResult Get(int id) => Ok();

    [HttpGet("search/{name?}")]                 // 可选参数
    public IActionResult Search(string? name = null) => Ok();
}

// 约定路由（传统 MVC）：集中配置
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

### 5.13 什么是 RESTful API？设计原则？

- REST：Representational State Transfer，基于 HTTP 资源的架构风格。
- 核心：资源 URL 命名（名词复数）、HTTP 方法语义（GET 查、POST 增、PUT 全量改、PATCH 部分改、DELETE 删）、状态码（200/201/204/400/401/403/404/409/500）、无状态、可缓存、统一接口。
- 加分：分页（`?page=1&size=20`）、过滤、排序、HATEOAS（可选）、版本化（URL/Header）。

### 5.14 什么是全局异常处理？如何实现？

- 目标：统一捕获未处理异常，避免敏感信息暴露给前端，统一响应格式，记录日志。
- 实现方式：
  1. **异常处理中间件**（`UseExceptionHandler` + 自定义 `IExceptionHandler`，.NET 8+）；
  2. `ExceptionFilterAttribute`（MVC 层）；
  3. 针对 `BadHttpRequestException` 等的专项处理。
- 要点：开发环境返回详细错误，生产环境返回统一 JSON 格式（错误码 + 消息 + traceId）。


**代码示例：**

```csharp
// .NET 8+ 推荐：实现 IExceptionHandler，统一处理未捕获异常
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex,
        CancellationToken ct)
    {
        _logger.LogError(ex, "未处理异常: {Path}", ctx.Request.Path);

        // 生产环境返回统一 JSON（不暴露内部细节）
        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await ctx.Response.WriteAsJsonAsync(new
        {
            code = 500,
            message = "服务器内部错误",
            traceId = ctx.TraceIdentifier
        }, ct);
        return true;    // 已处理，不再向上传播
    }
}

// 注册
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
app.UseExceptionHandler();   // 放在管道最前面

// MVC 层也可用 ExceptionFilterAttribute；开发环境用 UseDeveloperExceptionPage
```

### 5.15 Authentication 与 Authorization 的区别？（必问）

- **Authentication（认证）**：确认"你是谁"（登录验证身份）。
- **Authorization（授权）**：确认"你能做什么"（是否有权限访问）。
- 例子：登录成功 = 认证；登录后能否访问管理员接口 = 授权。
- ASP.NET Core：认证由 `AddAuthentication` + `[Authorize]` 触发；授权由 `AddAuthorization` + 策略/角色决定。

### 5.16 JWT 是什么？由哪几部分组成？为什么能防篡改？（高频）

**要点：**
- JWT（JSON Web Token）：无状态令牌，格式 `Header.Payload.Signature`（Base64Url 三段）。
  - **Header**：算法（HS256 等）与类型；
  - **Payload**：声明（claims：sub、exp、iss、自定义角色/用户信息）；
  - **Signature**：`HMACSHA256(header + "." + payload, secret)` 或 RSA 签名。
- 防篡改原理：签名用服务端密钥计算，任何改动导致验签失败。
- **验证要点**：验签名、校验 `exp` 过期、`iss`/`aud`；密钥安全存储。
- 缺点：无状态意味着无法主动注销/踢人（需黑名单或缩短有效期）；Payload 仅 Base64 编码非加密，**不要放敏感信息**。


**代码示例：**

```csharp
// 生成 JWT（服务端，登录成功后签发）
var claims = new[]
{
    new Claim(ClaimTypes.Name, "zhangsan"),
    new Claim("role", "Admin"),
    new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString())
};
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)); // secret 必须安全存储
var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

var token = new JwtSecurityToken(
    issuer: "MyApi", audience: "MyClient",
    claims: claims, expires: DateTime.UtcNow.AddHours(1),
    signingCredentials: creds);
string jwt = new JwtSecurityTokenHandler().WriteToken(token);   // Header.Payload.Signature

// 验证（中间件 AddAuthentication().AddJwtBearer() 自动完成）：
// 验签名 -> 校验 exp -> 校验 iss/aud
// 防篡改原理：Signature = HMACSHA256(header.payload, secret)，任何改动验签失败
// 注意：Payload 只是 Base64Url 编码非加密，不要放敏感信息

// Program.cs 配置
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ValidateIssuer = true, ValidIssuer = "MyApi",
        ValidateAudience = true, ValidAudience = "MyClient",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    });
```

### 5.17 Cookie 认证与 JWT 的区别？

| 维度 | Cookie 认证 | JWT |
|---|---|---|
| 状态 | 服务端会话（有状态） | 无状态 |
| 存储 | Cookie + 服务端 Session | 客户端持有 Token |
| 跨域/跨服务 | 不便 | 天然适合（网关、微服务） |
| 注销 | 立即失效 | 难立即失效 |
| 攻击面 | CSRF 风险 | XSS 窃取 Token 风险 |
| 适用 | 传统 Web 站点 | API、移动端、微服务 |

### 5.18 `[Authorize]` 与 `[AllowAnonymous]`？

- `[Authorize]`：要求已认证才能访问；`[Authorize(Roles = "Admin")]` 按角色；`[Authorize(Policy = "P")]` 按策略。
- `[AllowAnonymous]`：跳过认证（登录接口等）。
- 策略授权：`AddAuthorization` + `AddPolicy("P", p => p.RequireClaim(...))`；可组合角色、Claim、业务规则。


**代码示例：**

```csharp
[ApiController]
[Route("api/orders")]
[Authorize]                              // 整个控制器要求已认证
public class OrdersController : ControllerBase
{
    [HttpGet]
    public IActionResult List() => Ok(); // 需要认证

    [AllowAnonymous]                     // 跳过认证（登录接口等）
    [HttpPost("callback")]
    public IActionResult Callback() => Ok();

    [Authorize(Roles = "Admin")]         // 按角色
    [HttpDelete("{id}")]
    public IActionResult Delete(int id) => Ok();

    [Authorize(Policy = "AtLeast18")]    // 按策略
    [HttpGet("adult")]
    public IActionResult Adult() => Ok();
}

// 策略注册
builder.Services.AddAuthorization(o =>
    o.AddPolicy("AtLeast18", p => p.RequireClaim("age").RequireAssertion(c =>
        int.TryParse(c.User.FindFirst("age")?.Value, out var age) && age >= 18)));
```

### 5.19 基于角色授权与基于策略授权的区别？

- 角色授权：简单（Roles="Admin"），硬编码、粒度粗。
- 策略授权：基于 Claim/角色/业务条件组合，可集中配置、可单元测试、支持自定义 `IAuthorizationHandler`。
- 复杂权限系统（细粒度、动态权限）优先策略授权。

### 5.20 Session 与缓存（Cache）的区别？

- **Session**：保存**当前用户**的会话状态（登录信息、购物车），偏用户维度，与用户绑定。
- **Cache**：缓存**通用数据**提升性能（热点数据、字典项），不绑定用户。
- 一句话：Session 解决会话状态，Cache 解决性能。
- Session 默认内存（单实例），多实例需 `IDistributedCache`（Redis）支持。

### 5.21 `IMemoryCache` 与 `IDistributedCache` 的区别？

- `IMemoryCache`：进程内缓存，快但**多实例部署不一致**（每台机器各有一份）。
- `IDistributedCache`：分布式缓存（Redis、SQL Server），集群共享、支持序列化；API 是字节操作（需自行序列化），可配 `AddStackExchangeRedisCache`。
- 混合方案：本地缓存（L1）+ Redis（L2）两级缓存，注意一致性。


**代码示例：**

```csharp
// IMemoryCache：进程内缓存（单实例快；多实例部署每台各一份，不一致）
builder.Services.AddMemoryCache();

public class WeatherService
{
    private readonly IMemoryCache _cache;
    public WeatherService(IMemoryCache cache) => _cache = cache;

    public async Task<Weather> GetAsync(string city)
    {
        return await _cache.GetOrCreateAsync($"weather:{city}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.SlidingExpiration = TimeSpan.FromMinutes(1);   // 滑动过期
            return await FetchFromApi(city);
        });
    }
}

// IDistributedCache：分布式缓存（Redis），集群共享
builder.Services.AddStackExchangeRedisCache(o =>
    o.Configuration = "localhost:6379");

public class RedisCacheService
{
    private readonly IDistributedCache _cache;   // 字节操作，需自行序列化
    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task SetAsync(string key, string value)
    {
        await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
        { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
    }
}
static Task<Weather> FetchFromApi(string city) => Task.FromResult(new Weather());
class Weather { }
```

### 5.22 缓存穿透、击穿、雪崩分别是什么？如何解决？（高频必问）

**要点：**
- **穿透**：查询**不存在的数据**，缓存无 → 每次都打 DB。解决：缓存空值（短 TTL）；布隆过滤器前置过滤。
- **击穿**：某个**热点 key 过期**瞬间，大量请求同时打 DB。解决：互斥锁（只允许一个请求回源重建缓存）；逻辑过期；热点 key 不过期。
- **雪崩**：**大量 key 同时过期**或缓存服务宕机 → 请求洪峰打到 DB。解决：TTL 加随机值错开；多级缓存；限流降级；Redis 高可用（哨兵/集群）。
- 通用：限流 + 降级 + 监控告警。

### 5.23 Kestrel 是什么？与 IIS/Nginx 的关系？

- Kestrel：ASP.NET Core 内置的**跨平台高性能 Web 服务器**，直接处理 HTTP 请求。
- 部署时通常放在 IIS/Nginx 后面作**反向代理**：转发请求、HTTPS 终止、负载均衡、静态文件、安全防护。
- 反向代理与 Kestrel 之间可用 HTTP/2、Unix Socket 提升性能。
- 直接暴露 Kestrel 到公网可行但不推荐（缺代理层能力）。

### 5.24 什么是 CORS？如何配置？

- CORS（跨域资源共享）：浏览器同源策略的放宽机制，允许指定域名访问 API。
- 配置：`AddCors` + `UseCors` + `WithOrigins(...)`/`AllowAnyOrigin`。
- 注意：`AllowCredentials` 与 `AllowAnyOrigin` 不能同时使用；生产环境不要全开放；预检请求（OPTIONS）处理。


**代码示例：**

```csharp
// Program.cs
builder.Services.AddCors(o => o.AddPolicy("AllowWeb", policy =>
    policy.WithOrigins("https://myweb.example.com")    // 只允许指定域名
          .WithMethods("GET", "POST")                  // 限制方法
          .AllowCredentials()));                       // 允许携带凭证

var app = builder.Build();
app.UseCors("AllowWeb");      // 必须放在 UseAuthentication 之前

// 注意：
// 1. AllowCredentials 与 AllowAnyOrigin 不能同时使用
// 2. 生产环境不要 AllowAnyOrigin 全开放
// 3. 浏览器会先发 OPTIONS 预检请求，由中间件自动处理
```

### 5.25 ASP.NET Core 中如何做日志？

- 内置 `ILogger<T>`，级别：Trace < Debug < Information < Warning < Error < Critical。
- 结构化日志：模板化消息（`_logger.LogWarning("User {UserId} ...", id)`）输出 JSON 字段，便于检索。
- 可接 Serilog、Seq、ELK、OpenTelemetry 做集中式日志与链路追踪。
- 高性能日志：`LoggerMessage` 源生成（`[LoggerMessage]`）避免装箱与模板解析开销。


**代码示例：**

```csharp
// 注入 ILogger<T>，级别：Trace < Debug < Information < Warning < Error < Critical
public class OrderService
{
    private readonly ILogger<OrderService> _logger;
    public OrderService(ILogger<OrderService> logger) => _logger = logger;

    public async Task CreateAsync(long userId)
    {
        // 结构化日志：模板参数输出为 JSON 字段，便于检索
        _logger.LogInformation("创建订单 用户={UserId} 时间={At}",
            userId, DateTime.UtcNow);
    }
}

// 高性能日志：LoggerMessage 源生成（避免装箱与模板解析）
public static partial class Log
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "订单 {OrderId} 超时")]
    public static partial void OrderTimeout(ILogger logger, long orderId);
}

// 可接 Serilog、Seq、ELK、OpenTelemetry 做集中日志与链路追踪
// builder.Host.UseSerilog(...)
```

### 5.26 什么是 Health Check？如何实现？

- 健康检查：探活应用及依赖（DB、Redis、外部 API）状态，供负载均衡/容器编排（K8s liveness/readiness）使用。
- 实现：`AddHealthChecks()` + `MapHealthChecks("/health")` + `AddDbContextCheck`/`AddRedis` 等；可自定义 `IHealthCheck`。
- 区分 liveness（进程存活）与 readiness（能否接流量）。


**代码示例：**

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>()                    // 数据库探活
    .AddRedis("localhost:6379", "redis");                 // Redis 探活
    // .AddCheck<CustomCheck>("custom");                  // 自定义

var app = builder.Build();
app.MapHealthChecks("/health");                           // 基础探活（liveness）
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready")             // 就绪探针（readiness）
});

// K8s 配置：
// livenessProbe: 进程存活（/health）
// readinessProbe: 能否接流量（/health/ready，依赖 DB/Redis 都正常）

// 自定义 IHealthCheck：检查外部 API 等
public class CustomCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext ctx, CancellationToken ct)
    {
        await Task.Delay(1, ct);
        return HealthCheckResult.Healthy();
    }
}
```

### 5.27 什么是接口幂等性？如何实现？（高频）

- **幂等**：同一请求执行多次与执行一次效果相同（如重复支付不重复扣款）。
- 实现：
  1. 请求携带**唯一请求 ID**，服务端记录处理结果，重复请求直接返回首次结果；
  2. 数据库唯一约束兜底（如订单号）；
  3. 乐观锁/版本号；
  4. 消息队列场景：消费方按消息 ID 去重。
- 注意 HTTP 语义：GET/PUT/DELETE 天然幂等，POST 不幂等。


**代码示例：**

```csharp
// 方案1：唯一请求 ID + 服务端去重表
public class PaymentService
{
    private static readonly HashSet<string> Processed = new();   // 实际应存 DB

    public async Task<PayResult> PayAsync(string requestId, decimal amount)
    {
        lock (Processed)                                     // 多实例需用 Redis/DB 唯一约束
        {
            if (!Processed.Add(requestId))                   // 重复请求直接返回首次结果
                return new PayResult { Duplicated = true };
        }
        // 真正的支付逻辑（配合数据库唯一约束兜底：订单号 unique index）
        return new PayResult { Success = true };
    }
}

// 方案2：数据库唯一约束（如订单号）-> 重复插入抛异常，捕获后查原结果返回
// 方案3：乐观锁/版本号：UPDATE ... WHERE version = @v，影响行数 0 说明已被改

// HTTP 语义：GET/PUT/DELETE 天然幂等；POST 不幂等，需加请求 ID
record PayResult(bool Success = false, bool Duplicated = false);
```

### 5.28 什么是统一返回格式？

- 约定包装：`{ code, message, data }` 或 `{ success, error, data }`，配合全局异常处理和模型验证统一格式。
- 实现：`ActionResult` 包装器 + 过滤器/中间件统一序列化；注意与 OpenAPI 文档的兼容性。
- 避免：把业务错误码与 HTTP 状态码完全混用（可自定义 `ProblemDetails`）。


**代码示例：**

```csharp
// 统一包装：{ code, message, data }
public record ApiResult<T>(int Code, string Message, T? Data)
{
    public static ApiResult<T> Ok(T data) => new(0, "success", data);
    public static ApiResult<T> Fail(int code, string msg) => new(code, msg, default);
}

// 控制器返回统一格式
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ApiResult<UserDto>> Get(int id)
    {
        var user = await _service.GetAsync(id);
        if (user is null) return ApiResult<UserDto>.Fail(404, "用户不存在");
        return ApiResult<UserDto>.Ok(user);
    }
}

// 配合全局异常处理 + 模型验证过滤器统一序列化；
// 也可用 ProblemDetails（RFC 7807 标准）替代自定义格式
record UserDto(int Id, string Name);
```

### 5.29 Controller、Service、Repository 的职责是什么？为什么不建议业务逻辑写在 Controller？（高频）

- **Controller**：接收请求、参数校验、调用 Service、返回响应（薄）。
- **Service**：业务逻辑、事务边界、领域规则。
- **Repository**：数据访问抽象（可选，简单 CRUD 可直接用 DbContext）。
- **原因**：Controller 厚会导致难以单元测试、难以复用、职责混乱、代码膨胀；分层后每层可独立测试与替换。
- 加分：DDD 视角区分应用服务（协调）与领域服务（领域规则）。

### 5.30 什么是 SignalR？适用场景？

- SignalR：实时 Web 通信库（WebSocket 优先，降级 SSE/Long Polling），服务端主动推送。
- 核心：Hub（服务端）、`Client.SendAsync` 推送、群组、连接管理、`IHubContext` 从服务推。
- 场景：聊天、实时通知、看板、协同编辑、在线状态。


**代码示例：**

```csharp
// 1. 定义 Hub（服务端）
public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        // 广播给所有客户端
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task JoinGroup(string group)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
    }

    public override async Task OnConnectedAsync()   // 连接管理
    {
        await base.OnConnectedAsync();
    }
}

// 2. 注册
builder.Services.AddSignalR();
app.MapHub<ChatHub>("/chatHub");

// 3. 服务端主动推送（从任意服务注入 IHubContext）
public class NotifyService
{
    private readonly IHubContext<ChatHub> _hub;
    public NotifyService(IHubContext<ChatHub> hub) => _hub = hub;

    public async Task PushAsync(string user, string msg)
        => await _hub.Clients.User(user).SendAsync("Notify", msg);
}

// 4. 客户端（JS）：const conn = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
// 场景：聊天、实时通知、看板、协同编辑、在线状态
```

### 5.31 什么是 gRPC？与 REST 的区别？

- gRPC：基于 HTTP/2 + Protobuf 的 RPC 框架，二进制序列化、双向流、强类型契约（`.proto`）。
- 与 REST 对比：性能更高、契约强、适合服务间通信；调试不如 REST 直观、浏览器支持弱（需 gRPC-Web）。
- .NET 场景：微服务内部调用首选 gRPC，外部 API 用 REST。

### 5.32 什么是限流（Rate Limiting）？如何实现？

- 限制客户端请求速率，防滥用与雪崩。
- .NET 7+ 内置：`AddRateLimiter` + 固定窗口/滑动窗口/令牌桶/并发限制策略；`EnableRateLimiting` 特性。
- 也可用网关层（Nginx、Ocelot、YARP）或 Redis 计数器实现分布式限流。


**代码示例：**

```csharp
// .NET 7+ 内置限流
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.OnRejected = async (ctx, ct) =>
    {
        ctx.HttpContext.Response.Headers.RetryAfter = "60";
        await Task.CompletedTask;
    };

    // 固定窗口：每分钟每 IP 最多 100 次
    o.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
    });

    // 令牌桶：每秒补充 10 个令牌，突发 20
    o.AddTokenBucketLimiter("token", opt =>
    {
        opt.TokenLimit = 20;
        opt.TokensPerPeriod = 10;
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
    });
});

app.UseRateLimiter();

// 控制器/Action 上应用
[EnableRateLimiting("fixed")]
[HttpGet]
public IActionResult Get() => Ok();
// [DisableRateLimiting] 可豁免
// 分布式限流：Redis INCR + Lua 或网关（Nginx/Ocelot/YARP）
```

### 5.33 什么是多租户（Multi-Tenancy）？（高级）

- 多租户：一套应用服务多个客户，数据隔离（共享库/共享 schema/独立库）。
- .NET 实现：按请求（域名/Header）解析租户 ID → 动态切换 `ConnectionString`/`DbContext` 或全局查询过滤器（`HasQueryFilter`）隔离数据。
- 注意：连接池 key、缓存 key 需包含租户维度，防止数据交叉。

### 5.34 什么是 `ProblemDetails`？

- 标准化的错误响应格式（RFC 7807）：`{ type, title, status, detail, instance, errors }`。
- ASP.NET Core 内置支持，`[ApiController]` 自动返回 400/404 等 ProblemDetails；利于 API 客户端统一解析错误。

### 5.35 什么是 `IHttpClientFactory`？为什么要用它？（高频）

- 工厂模式管理 `HttpClient`：**避免 `new HttpClient()` 导致的 Socket 耗尽**（旧 HttpClient 不释放底层连接）。
- 提供：命名/类型化客户端、策略（结合 Polly 重试/熔断）、`HttpMessageHandler` 生命周期管理、日志。
- 用法：`AddHttpClient` + 注入 `IHttpClientFactory` 创建客户端。


**代码示例：**

```csharp
// 错误：每次 new HttpClient() -> 底层 Socket 耗尽（TIME_WAIT 堆积）
for (int i = 0; i < 1000; i++)
{
    using var client = new HttpClient();     // 每次新建，连接不重用
}

// 正确：IHttpClientFactory 管理 handler 生命周期（默认 2 分钟轮换）
builder.Services.AddHttpClient("Github", c =>
{
    c.BaseAddress = new Uri("https://api.github.com/");
    c.Timeout = TimeSpan.FromSeconds(30);
    c.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/1.0");
})
.AddRetryPolicy();   // 可配合 Polly（见 9.18）

// 注入使用
public class GithubService
{
    private readonly IHttpClientFactory _factory;
    public GithubService(IHttpClientFactory factory) => _factory = factory;

    public async Task<string> GetRepoAsync(string name)
    {
        var client = _factory.CreateClient("Github");   // 复用连接池
        return await client.GetStringAsync($"/repos/{name}");
    }
}

// 也可用类型化客户端（AddHttpClient<GithubService>()）——强类型注入
```

### 5.36 什么是后台任务（Hosted Service）？如何避免常见陷阱？

- `BackgroundService` 实现长期后台循环（队列消费、定时扫描）。
- **陷阱**：构造函数注入 Scoped 服务（应为 `IServiceScopeFactory` 每次循环创建 Scope，避免 captive dependency、ChangeTracker 无限增长）；正确处理 `CancellationToken`；异常捕获防止服务静默退出。


**代码示例：**

```csharp
// 后台任务：队列消费、定时扫描等长期循环
public class OrderProcessor : BackgroundService
{
    // 陷阱1：不要直接注入 Scoped 服务（BackgroundService 是 Singleton 生命周期）
    // private readonly AppDbContext _db;   // 错误！会导致 Captive Dependency
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderProcessor> _logger;

    public OrderProcessor(IServiceScopeFactory sf, ILogger<OrderProcessor> l)
    {
        _scopeFactory = sf;
        _logger = l;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 每次循环创建 Scope，拿到新的 DbContext（避免 ChangeTracker 无限增长）
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await ProcessNextAsync(db, stoppingToken);
            }
            catch (Exception ex)          // 陷阱2：捕获异常防止服务静默退出
            {
                _logger.LogError(ex, "处理失败");
                await Task.Delay(1000, stoppingToken);   // 退避后重试
            }
        }
    }
}
```

### 5.37 什么是端点路由（Endpoint Routing）？

- .NET 3+：路由与中间件分离，`UseRouting` 匹配端点（不执行），`UseEndpoints`/`MapControllers` 执行。
- 好处：中间件可同时服务 MVC、SignalR、gRPC、Health Check 等多种端点；`LinkGenerator` 生成 URL。

### 5.38 什么是 `Options` 验证与 `ValidateOnStart`？

- `[ValidateOnStart]`：应用启动时立即验证配置，避免配置错误延迟到请求期才发现。
- 数据注解 + `IOptionsValidateOptions` 自定义校验。


**代码示例：**

```csharp
public class SmtpOptions
{
    [Required]
    public string Host { get; set; } = "";
    [Range(1, 65535)]
    public int Port { get; set; } = 25;
    public string? Username { get; set; }
}

// 绑定 + 启用启动时验证
builder.Services.AddOptions<SmtpOptions>()
    .Bind(builder.Configuration.GetSection("Smtp"))
    .ValidateDataAnnotations()                 // 数据注解验证
    .Validate(o => o.Port != 25 || o.Username != null,   // 自定义规则
             "使用默认端口时必须有用户名")
    .ValidateOnStart();                        // 启动即验证，配置错误立刻暴露

// 也可实现 IValidateOptions<T> 做复杂校验（跨配置项、依赖服务）
```

### 5.39 什么是响应缓存与 HTTP 缓存？

- `[ResponseCache]` + `AddResponseCaching`：按 `Cache-Control` 缓存响应；配合 CDN/浏览器缓存。
- 注意缓存键（Query 参数）、用户相关数据不能缓存；`Vary` 头控制。

### 5.40 什么是静态文件中间件？

- `UseStaticFiles` 提供 wwwroot 静态资源；`UseDefaultFiles`、`UseDirectoryBrowser`（慎用）。
- 生产环境建议交给反向代理/CDN 处理静态资源。

### 5.41 什么是 HTTPS 重定向与 HSTS？

- `UseHttpsRedirection`：HTTP → HTTPS 301/308。
- HSTS：`UseHsts` 通知浏览器强制 HTTPS（仅生产环境）。
- 开发环境跳过；注意与反向代理后协议头（`ForwardedHeaders`）配合。

### 5.42 什么是 `ForwardedHeaders`？

- 应用部署在反向代理后，需信任 `X-Forwarded-For`/`X-Forwarded-Proto` 才能拿到真实客户端 IP 与协议。
- `UseForwardedHeaders` + `KnownProxies` 配置；防止伪造头。

### 5.43 什么是数据保护（Data Protection）？

- ASP.NET Core 内置加密栈：加密 Cookie、防伪令牌、临时数据。
- `AddDataProtection` + 密钥存储（文件、Azure Key Vault），多实例需共享密钥环。

### 5.44 什么是防伪令牌（Antiforgery）与 CSRF？

- CSRF：跨站请求伪造——诱导已登录用户在不知情下发起请求。
- 防御：`[AutoValidateAntiforgeryToken]`/`[ValidateAntiForgeryToken]` + Cookie 中令牌与表单/Header 中令牌配对校验。
- API 场景：`X-CSRF-TOKEN` Header + SameSite Cookie。


**代码示例：**

```csharp
// 1. 注册
builder.Services.AddAntiforgery(o => o.HeaderName = "X-CSRF-TOKEN");

// 2. MVC 页面（Razor）：表单自动生成隐藏令牌
// <form asp-action="Delete" method="post">
//     @Html.AntiForgeryToken()
//     ...
// </form>
// [AutoValidateAntiforgeryToken] 全局校验（POST/PUT/DELETE 自动）

// 3. API 场景：客户端从 GET 获取令牌，请求时放 Header
app.Use(async (ctx, next) =>
{
    var af = ctx.RequestServices.GetRequiredService<IAntiforgery>();
    var tokens = af.GetAndStoreTokens(ctx);
    ctx.Response.Headers["X-CSRF-TOKEN"] = tokens.RequestToken!;
    await next();
});

// 原理：Cookie 中令牌 与 表单/Header 中令牌 配对校验
// 附加：SameSite=Cookie 减少 CSRF 面；CSRF 与 XSS 不同（见 5.45）
```

### 5.45 什么是 XSS？ASP.NET Core 如何防御？

- XSS：注入恶意脚本。防御：输出编码（Razor 默认 `@` 编码）、`HtmlEncoder`、CSP 头、HttpOnly Cookie、输入校验。
- 不要用 `@Html.Raw` 处理用户输入。

---

<a id="c6"></a>

## 六、Entity Framework Core

### 6.1 DbContext 的作用是什么？为什么一般注册为 Scoped？（必问）

**要点：**
- `DbContext` 是 EF Core 核心：管理连接、跟踪实体状态（ChangeTracker）、执行 LINQ 查询、保存变更（`SaveChanges`）、事务。
- **Scoped 原因**：一个 HTTP 请求对应一个工作单元，同请求共享同一上下文，便于事务与实体跟踪；Singleton 会带来**线程不安全**、ChangeTracker 无限增长、连接长期占用。
- 注意：DbContext 构造函数注入 IOptions 等；多线程下不可共用。


**代码示例：**

```csharp
// DbContext：管理连接、实体状态跟踪、查询、保存、事务（工作单元）
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Customer> Customers => Set<Customer>();
}

// 注册为 Scoped：一个 HTTP 请求一个上下文，同请求共享，便于事务与跟踪
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// 错误：注册为 Singleton -> 多请求共享一个 DbContext
// 导致线程不安全、ChangeTracker 无限增长、连接长期占用
// 后台任务请用 AddDbContextFactory<T> 手动创建
class Order { public long Id { get; set; } }
class Customer { public long Id { get; set; } }
```

### 6.2 什么是 Code First 与 Database First？

- **Code First**：先写实体类，`Add-Migration` 生成迁移，`Update-Database` 创建/更新库；现代 .NET 默认。
- **Database First**：从已有库生成实体（Scaffold-DbContext）；适合存量库。
- 注意：迁移是版本化的 schema 变更管理，不可删除已发布的迁移。

### 6.3 什么是 Migration？常用命令？（必问）

- Migration：把模型变更翻译为数据库结构变更的脚本与快照，保证代码与库同步。
- 命令：`dotnet ef migrations add Xxx` → 生成 `Up()`/`Down()` + 快照；`dotnet ef database update` 应用；`dotnet ef migrations remove` 撤销未应用的。
- 生产发布：在 CI/CD 中以独立步骤执行 `database update`，避免多实例同时启动时竞争（不要依赖启动时 `Migrate()`）。


**代码示例：**

```csharp
// 1. 添加迁移（生成 Up()/Down() + 模型快照）
// dotnet ef migrations add AddOrderTable

// 生成的迁移类
public partial class AddOrderTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Orders",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                CustomerId = table.Column<long>(type: "bigint", nullable: false),
                Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Orders", x => x.Id));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
        => migrationBuilder.DropTable(name: "Orders");
}

// 2. 应用迁移：dotnet ef database update
// 3. 其他：dotnet ef migrations remove（撤销未应用）
//        dotnet ef migrations list（查看列表）
//        dotnet ef database update Xxx（回滚到指定版本，执行 Down）
// 生产：在 CI/CD 中独立执行 database update，不要依赖启动时 Migrate()
```

### 6.4 什么是 Change Tracking？实体状态有哪些？

- ChangeTracker 跟踪实体状态：`Added`、`Modified`、`Unchanged`、`Deleted`、`Detached`。
- `SaveChanges` 根据状态生成 INSERT/UPDATE/DELETE；支持 `Reload`、`Detach`。
- 追踪查询（默认）每行实体缓存快照，用于生成 UPDATE 的 SET 子句。


**代码示例：**

```csharp
// 实体状态：Added / Modified / Unchanged / Deleted / Detached
using var db = new AppDbContext();

var order = new Order { Id = 1, Total = 100m };
db.Orders.Add(order);                        // Added：将生成 INSERT
Console.WriteLine(db.Entry(order).State);    // Added

var tracked = await db.Orders.FirstAsync();  // 追踪查询（默认），状态 Unchanged
tracked.Total = 200m;                        // 修改后自动变 Modified（快照对比）
Console.WriteLine(db.Entry(tracked).State);  // Modified

db.Orders.Remove(tracked);                   // Deleted：将生成 DELETE

await db.SaveChangesAsync();                 // 根据状态统一生成 SQL 并执行
// 其他：Reload()（重载最新值）、Detach（脱离跟踪）、DetectChanges
```

### 6.5 什么是 `AsNoTracking()`？什么时候用？（高频）

- 查询结果**不追踪**，减少内存与快照开销，提升查询性能。
- 适用：**只读查询**（报表、列表展示）；不追踪的实体不能直接 `SaveChanges` 更新。
- 对比：需要更新的实体要追踪（或用 `Update` 附加）；`AsNoTrackingWithIdentityResolution` 处理导航属性重复。


**代码示例：**

```csharp
// 只读查询不追踪：省内存、免快照对比，性能更好
var list = await db.Orders
    .AsNoTracking()
    .Where(o => o.Status == "Paid")
    .ToListAsync();

// 需要更新时：追踪查询 或 Update 附加
var order = await db.Orders.FirstAsync(o => o.Id == 1);   // 默认追踪
order.Total = 300m;
await db.SaveChangesAsync();

// 不追踪查询后更新：手动 Update 附加（整个实体标记 Modified）
var stale = await db.Orders.AsNoTracking().FirstAsync(o => o.Id == 1);
stale.Total = 500m;
db.Orders.Update(stale);
await db.SaveChangesAsync();

// AsNoTrackingWithIdentityResolution：处理导航属性去重（避免重复加载同一实体）
```

### 6.6 什么是延迟执行（Deferred Execution）？查询什么时候真正执行？（高频）

**要点：**
- 构建 `IQueryable` 不执行 SQL；**遍历/物化**时才执行。
- 触发执行的操作：`ToList()`/`ToArray()`/`ToDictionary()`、`First()`/`Single()`/`Count()`/`Any()`/`Max()` 等聚合、`foreach`、`async` 版本（`ToListAsync`）。
- 意义：可分段组合查询条件（`if` 加 Where）；**陷阱**：延迟执行在循环内重复枚举会导致重复查询；查询在迭代时才执行，注意 using 作用域。


**代码示例：**

```csharp
// 构建 IQueryable 不执行 SQL；遍历/物化时才执行
IQueryable<Order> query = db.Orders.Where(o => o.Status == "Paid");

// 可继续组合（仍不执行）
if (onlyLarge) query = query.Where(o => o.Total > 1000);

var result = await query.ToListAsync();      // 此刻才真正发 SQL

// 触发执行的操作：
await db.Orders.ToListAsync();               // 物化
await db.Orders.FirstAsync();                // 单条
await db.Orders.CountAsync();                // 聚合
await foreach (var o in db.Orders) { }       // 遍历

// 陷阱：循环内重复枚举会重复查询
foreach (var o in db.Orders) { }             // 查询1
foreach (var o in db.Orders) { }             // 查询2（又查一次！）
```

### 6.7 什么是 N+1 查询？如何避免？（必问）

**要点：**
- N+1：先查 1 条主记录，再对每条记录各发 1 条查询加载导航属性（1000 条订单 = 1001 条 SQL）。
- **解决**：
  1. **贪婪加载（Eager Loading）**：`Include(o => o.Customer)`、`ThenInclude(o => o.Customer.Address)`；
  2. **显式加载（Explicit Loading）**：`Entry(x).Reference(...).LoadAsync()`；
  3. **投影（Projection）**：`Select` 只取需要的字段（推荐，避免整表字段）。
- **懒加载（Lazy Loading）**：默认关闭，需装包 + `virtual` 导航属性；生产环境**谨慎使用**（隐式触发 N+1）。


**代码示例：**

```csharp
// 错误写法（N+1）：查 1 条主记录 + 每条记录各查 1 次导航属性
var orders = await db.Orders.ToListAsync();       // 1 条 SQL
foreach (var o in orders)
{
    Console.WriteLine(o.Customer.Name);           // 每条都触发 1 条查询（1000 条 = 1000 次！）
}

// 解决1：贪婪加载 Include / ThenInclude（一次 JOIN）
var withCustomers = await db.Orders
    .Include(o => o.Customer)
    .ToListAsync();                               // 1 条 JOIN SQL

// 解决2：投影（推荐，只取需要的字段）
var dtos = await db.Orders
    .Select(o => new { o.Id, CustomerName = o.Customer.Name })
    .ToListAsync();                               // 1 条 SQL

// 懒加载：默认关闭，需装包 + virtual 导航属性，生产环境谨慎使用（隐式 N+1）
```

### 6.8 `Include` 与 `ThenInclude` 的作用？

- `Include`：加载一级导航属性（JOIN）。
- `ThenInclude`：加载下一级（`Include(o => o.Items).ThenInclude(i => i.Product)`）。
- 过滤 Include（EF Core 5+）：`Include(o => o.Items.Where(i => i.Qty > 1))`。


**代码示例：**

```csharp
// Include：加载一级导航属性（JOIN）
var orders = await db.Orders
    .Include(o => o.Customer)
    .ToListAsync();

// ThenInclude：加载下一级导航
var orders2 = await db.Orders
    .Include(o => o.Customer)               // Order -> Customer
        .ThenInclude(c => c.Address)        // Customer -> Address（两级）
    .Include(o => o.Items)                  // 多个 Include 并列
        .ThenInclude(i => i.Product)        // Order -> Items -> Product
    .ToListAsync();

// 过滤 Include（EF Core 5+）：只加载满足条件的导航集合
var orders3 = await db.Orders
    .Include(o => o.Items.Where(i => i.Qty > 1))
    .ToListAsync();
```

### 6.9 什么是 `AsSplitQuery()`？什么时候用？

- 默认单条 JOIN 大查询；`AsSplitQuery` 拆成多条查询（EF Core 5+），减少笛卡尔爆炸与结果集膨胀。
- 适用：多级 Include、一对多关系多时；代价是多次往返，需权衡。
- EF Core 8+ 有 `SplitQueryBehavior` 全局配置。

### 6.10 什么是投影查询（Projection）？为什么要用？

- `Select(x => new Dto { ... })` 只取需要的列，SQL 更小、避免整实体加载、减少传输量。
- 投影后是 DTO（匿名类型/记录），不追踪更高效。
- 陷阱：投影中调用客户端方法会导致"客户端评估"错误或性能差（EF Core 3+ 默认禁止隐式客户端评估）。


**代码示例：**

```csharp
// 投影：Select 只取需要的列，SQL 更小、避免整实体加载
var result = await db.Orders
    .Where(o => o.Status == "Paid")
    .Select(o => new OrderDto
    {
        Id = o.Id,
        CustomerName = o.Customer!.Name,          // JOIN 取关联字段
        Total = o.Total
    })
    .ToListAsync();                               // SQL: SELECT o.Id, c.Name, o.Total

// 匿名类型投影（不追踪更高效）
var light = await db.Orders
    .Select(o => new { o.Id, o.Total })
    .ToListAsync();

// 陷阱：投影中调用自定义客户端方法会导致"客户端评估"错误（EF Core 3+ 默认禁止）
record OrderDto { public long Id { get; set; } public string? CustomerName { get; set; } public decimal Total { get; set; } }
```

### 6.11 什么是并发控制？乐观并发如何实现？（高频）

**要点：**
- 并发控制解决多人同时修改同一行数据。
- **悲观并发**：`SELECT ... FOR UPDATE`/行锁，先锁后改，冲突少但并发低。
- **乐观并发**：修改时校验版本，冲突则抛 `DbUpdateConcurrencyException`。
  - 实现：`[Timestamp]`/`IsRowVersion()` 的 `byte[] RowVersion` 或 `IsConcurrencyToken()` 标记列；EF 生成 `WHERE ... AND RowVersion = @v`，受影响行数为 0 时冲突。
- 处理：捕获异常 → 读取最新值 → 提示用户重试或合并。


**代码示例：**

```csharp
// 实体：RowVersion 并发令牌（每行一个版本号，更新时自动递增）
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Stock { get; set; }
    [Timestamp]                                 // 或 Fluent: IsRowVersion()
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

// 更新时 EF 生成：UPDATE ... SET ... WHERE Id=@id AND RowVersion=@oldVersion
// 受影响行数为 0 -> 被其他事务修改 -> 抛 DbUpdateConcurrencyException
try
{
    product.Stock -= 1;
    await db.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException ex)
{
    // 冲突处理：读取最新值，提示用户重试或合并
    foreach (var entry in ex.Entries)
    {
        await entry.ReloadAsync();              // 重新加载数据库最新值
    }
    Console.WriteLine("并发冲突，请重试");
}
// 也可用并发令牌列（IsConcurrencyToken）标记普通列，如 int Version
```

### 6.12 什么是软删除与全局查询过滤器？（高频）

- **软删除**：不物理删除，加 `IsDeleted` 标志，`Update` 置位。
- **全局查询过滤器**：`modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted)`——所有查询自动过滤，且不能忘记（防止查回已删数据）；也可用于**多租户**隔离。
- 注意：过滤器对 `Include` 导航也生效；需要绕过时用 `IgnoreQueryFilters()`。


**代码示例：**

```csharp
public class Order
{
    public long Id { get; set; }
    public bool IsDeleted { get; set; }        // 软删除标志
}

// 配置全局查询过滤器：所有查询自动过滤已删除数据
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
}

// 使用：普通查询自动排除已删除
var active = await db.Orders.ToListAsync();    // WHERE IsDeleted = 0

// 软删除操作：IgnoreQueryFilters 绕过过滤器查被删数据
var order = await db.Orders.IgnoreQueryFilters()
    .FirstAsync(o => o.Id == 1);
order.IsDeleted = true;                        // 置位而非物理删除
await db.SaveChangesAsync();

// 用途：数据保留/审计；也可用于多租户隔离：
// HasQueryFilter(e => e.TenantId == _tenantId)
```

### 6.13 什么是影子属性（Shadow Property）？

- 不在实体类中定义、但存在于模型中的属性（EF 内部维护），如外键、`IsDeleted`、租户 ID。
- 访问：`context.Entry(entity).Property("TenantId").CurrentValue`。
- 用途：多租户、审计字段、不暴露于实体的列。

### 6.14 什么是数据种子（Seeding）？有哪几种方式？

- 初始化参考数据（国家代码、角色、字典项）。
- 方式：
  1. **模型种子**（`HasData`）：随迁移生成，版本化、随 schema 发布；适合静态参考数据；
  2. **运行时种子**：启动时 `EnsureCreated`/`Migrate` 后幂等检查插入；适合依赖服务/动态数据。
- 幂等性：加"已存在则跳过"判断。


**代码示例：**

```csharp
// 方式1：模型种子 HasData（随迁移生成，版本化、随 schema 发布）
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Role>().HasData(
        new Role { Id = 1, Name = "Admin" },
        new Role { Id = 2, Name = "User" });
}

// 方式2：运行时种子（启动时幂等检查插入，适合依赖服务/动态数据）
public class SeedWorker : IHostedService
{
    private readonly IServiceScopeFactory _sf;
    public SeedWorker(IServiceScopeFactory sf) => _sf = sf;

    public async Task StartAsync(CancellationToken ct)
    {
        using var scope = _sf.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!await db.Roles.AnyAsync(r => r.Name == "Admin", ct))
        {
            db.Roles.Add(new Role { Name = "Admin" });
            await db.SaveChangesAsync(ct);   // 幂等：已存在则跳过
        }
    }
    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
class Role { public int Id { get; set; } public string Name { get; set; } = ""; }
```

### 6.15 什么是 `SaveChanges` 事务？如何显式使用事务？

- `SaveChanges` 默认在事务中执行多个变更（同上下文）。
- 显式：`context.Database.BeginTransaction()` + `Commit`/`Rollback`；`CreateExecutionStrategy` 处理**连接级重试**（`EnableRetryOnFailure`）。
- 跨库/跨服务事务：EF 的本地事务不能跨库（需分布式方案：本地消息表、Outbox、Saga）。


**代码示例：**

```csharp
// SaveChanges 默认在事务中执行同上下文的多个变更
using var db = new AppDbContext();

db.Orders.Add(new Order { Total = 100m });
db.Customers.Add(new Customer { Name = "张三" });
await db.SaveChangesAsync();          // 一个事务：要么全成功要么全失败

// 显式事务：控制提交/回滚时机
await using var tx = await db.Database.BeginTransactionAsync();
try
{
    db.Orders.Add(new Order { Total = 200m });
    await db.SaveChangesAsync();

    await db.Database.ExecuteSqlAsync(
        $"UPDATE Accounts SET Balance = Balance - 200 WHERE Id = 1");

    await tx.CommitAsync();           // 提交
}
catch
{
    await tx.RollbackAsync();         // 回滚
    throw;
}

// 连接级重试：EnableRetryOnFailure 配合执行策略（CreateExecutionStrategy）
// 跨库事务需分布式方案（Outbox、Saga），EF 本地事务不能跨库
```

### 6.16 什么是 `ExecuteUpdate` / `ExecuteDelete`？（EF Core 7+）

- 直接生成 UPDATE/DELETE SQL，**不加载实体**，避免先查后改；性能大幅提升。
- 条件 + 表达式：`context.Users.Where(u => u.Inactive).ExecuteDelete()`。
- 对比：`SaveChanges` 适合少量实体与复杂状态流转。


**代码示例：**

```csharp
// ExecuteDelete：直接生成 DELETE SQL，不加载实体（性能大幅提升）
int deleted = await db.Orders
    .Where(o => o.Status == "Canceled" && o.CreatedAt < cutoff)
    .ExecuteDeleteAsync();                     // DELETE ... WHERE ...

// ExecuteUpdate：直接生成 UPDATE SQL，不加载实体
int updated = await db.Products
    .Where(p => p.Stock < 10)
    .ExecuteUpdateAsync(setters =>
        setters.SetProperty(p => p.Stock, 0)          // 直接置 0
               .SetProperty(p => p.Status, "SoldOut"));
// UPDATE Products SET Stock = 0, Status = 'SoldOut' WHERE Stock < 10

// 对比：SaveChanges 适合少量实体与复杂状态流转（需跟踪）
// ExecuteUpdate/Delete 适合批量操作，无法触发 SaveChanges 的变更通知
```

### 6.17 什么是编译查询（Compiled Query）？

- `EF.CompileQuery` / `EF.CompileAsyncQuery`：把查询表达式预编译，避免每次重新翻译 SQL。
- 高频固定查询场景收益明显；EF Core 8+ 有**自动编译查询**优化（部分场景自动）。

### 6.18 EF Core 性能优化有哪些手段？（高级）

1. 只读查询用 `AsNoTracking`；
2. 投影 `Select` 只取所需列；
3. 避免 N+1（Include/投影）；
4. 大数据量分页用**键集分页**（`WHERE id > @last ORDER BY id`）而非 `Skip/Take`（后者 OFFSET 性能差）；
5. 批量操作用 `ExecuteUpdate/Delete`；
6. 必要索引（迁移中 `HasIndex` + 查询字段）；
7. `AsSplitQuery` 控制 JOIN 膨胀；
8. 编译查询/自动编译；
9. 连接池（`Pooling`，默认开启，注意 DbContext 无状态）；
10. 关闭 ChangeTracker 不必要的导航修复（`AutoDetectChangesEnabled`）。

### 6.19 什么是连接池（Connection Pooling）？

- ADO.NET/EF Core 默认启用连接池：连接关闭后回池复用，避免每次新建 TCP + 认证开销。
- 注意：DbContext 未释放会占用连接；`Async` 方法 + 及时释放；连接池峰值限制 `Max Pool Size`。

### 6.20 `AddDbContext` 生命周期与工厂模式？

- `AddDbContext<T>` 默认 Scoped；控制台/后台任务用 `AddDbContextFactory<T>` 每次手动创建上下文。
- 注册时配置 `UseSqlServer(conn)`、`EnableSensitiveDataLogging`（仅开发）、`EnableRetryOnFailure`。

### 6.21 什么是导航属性？一对多/多对多如何配置？

- 导航属性：实体间关系引用（`Customer.Orders`）。
- 一对多：`HasMany(o => o.Items).WithOne(i => i.Order).HasForeignKey(i => i.OrderId)`。
- 多对多（EF Core 5+）：`HasMany(a => b).WithMany()` 自动生成连接表。
- 级联删除（`Cascade`/`Restrict`）按需配置。


**代码示例：**

```csharp
public class Order
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }                  // 导航属性
    public List<OrderItem> Items { get; set; } = new();      // 一对多集合
}

public class Customer
{
    public long Id { get; set; }
    public List<Order> Orders { get; set; } = new();
}

public class Student { public int Id { get; set; } public List<Course> Courses { get; set; } = new(); }
public class Course  { public int Id { get; set; } public List<Student> Students { get; set; } = new(); }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // 一对多：显式配置（约定可自动发现）
    modelBuilder.Entity<Order>()
        .HasOne(o => o.Customer)
        .WithMany(c => c.Orders)
        .HasForeignKey(o => o.CustomerId)
        .OnDelete(DeleteBehavior.Cascade);                  // 级联删除按需

    // 多对多（EF Core 5+）：自动生成连接表 StudentCourse
    modelBuilder.Entity<Student>()
        .HasMany(s => s.Courses)
        .WithMany(c => c.Students);
}
```

### 6.22 什么是 `Owned` 类型（值对象映射）？

- `Owned Entity`：值对象（如 `Address`、`Money`）嵌入同一张表（或按类型拆表）。
- 配置：`OwnsOne`/`OwnsMany`；适合 DDD 值对象建模。

### 6.23 什么是 `Migrations` 的回滚？

- `dotnet ef database update PreviousMigration` 回滚到指定版本（执行 `Down()`）；注意破坏性变更（删列）的逆操作。

### 6.24 什么是数据注解 vs Fluent API？

- 数据注解：实体上 `[Key]`、`[Required]`、`[MaxLength]`、`[Index]`，简单直接。
- Fluent API：`OnModelCreating` 中集中配置，灵活（关系、索引、并发令牌、过滤器）；复杂模型推荐 Fluent API。


**代码示例：**

```csharp
// 数据注解：实体上直接标注，简单直接
public class Product
{
    [Key]                                  // 主键
    public int Id { get; set; }

    [Required, MaxLength(100)]             // 非空 + 长度
    public string Name { get; set; } = "";

    [Column(TypeName = "decimal(18,2)")]   // 列类型
    public decimal Price { get; set; }

    [Index(nameof(Code))]                  // 索引（EF Core 5+）
    public string Code { get; set; } = "";
}

// Fluent API：OnModelCreating 集中配置，灵活（关系、索引、并发令牌、过滤器）
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>(e =>
    {
        e.HasKey(p => p.Id);
        e.Property(p => p.Name).IsRequired().HasMaxLength(100);
        e.Property(p => p.Price).HasColumnType("decimal(18,2)");
        e.HasIndex(p => p.Code);
    });
}
// 复杂模型（多对多、值对象、全局过滤器）推荐 Fluent API；简单字段用注解
```

### 6.25 什么是 `DbSet<T>`？

- `DbSet<T>` 代表实体集合，是查询与增删改的入口：`context.Users.Add(...)`、`context.Users.Where(...)`。

---

<a id="c7"></a>

## 七、数据库与缓存

### 7.1 什么是索引？聚集索引与非聚集索引的区别？（高频）

**要点：**
- 索引是排序的数据结构（B-Tree），加速查询，代价是写慢、占空间。
- **聚集索引（Clustered）**：表数据**物理存储顺序**按索引键排列；每表只有一个；主键默认聚集；范围查询/顺序访问快。
- **非聚集索引（Non-Clustered）**：索引结构存键 + 指向行的指针（回表）；一个表可有多个；覆盖索引（INCLUDE 列）避免回表。
- 复合索引：最左前缀原则（`(a, b, c)` 可用于 `a`、`a,b` 查询，不能直接用于 `b`）。
- 建索引注意：选择性高、查询频繁的列；避免过度索引拖慢写入。


**代码示例（SQL Server）：**

```sql
-- 聚集索引：表数据物理存储顺序按索引键排列，每表一个（主键默认）
CREATE TABLE Orders
(
    Id        BIGINT IDENTITY PRIMARY KEY,        -- 默认聚集索引
    OrderNo   NVARCHAR(32) NOT NULL,
    CustomerId BIGINT NOT NULL,
    Total     DECIMAL(18,2) NOT NULL
);

-- 非聚集索引：存键 + 行指针（回表）；一个表可有多个
CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);

-- 覆盖索引：INCLUDE 避免回表（查询只需这些列时）
CREATE INDEX IX_Orders_CustomerId_Total ON Orders(CustomerId)
    INCLUDE (Total);

-- 复合索引：最左前缀原则 (a, b, c) 可用于 a、a,b，不能直接用于 b
CREATE INDEX IX_Orders_CustomerId_CreatedAt ON Orders(CustomerId, CreatedAt);

-- 查看执行计划是否命中索引（Index Seek vs Table Scan）
SET SHOWPLAN_ALL ON;
SELECT * FROM Orders WHERE CustomerId = 100;
```

### 7.2 什么是 SARGable？为什么函数包裹列会走不到索引？

- SARGable：查询条件**可利用索引**（Index Seek）。
- **非 SARGable**：`WHERE YEAR(OrderDate) = 2024`、`WHERE Name LIKE '%xx%'`、`WHERE col + 1 = 10`——列被函数/运算包裹，无法用索引，退化为全表扫描。
- 改写：`WHERE OrderDate >= '2024-01-01' AND OrderDate < '2025-01-01'`；`LIKE 'xx%'` 可走索引。


**代码示例（SQL Server）：**

```sql
-- 非 SARGable：列被函数/运算包裹，无法用索引，退化为全表扫描
SELECT * FROM Orders WHERE YEAR(CreatedAt) = 2024;      -- 不走索引
SELECT * FROM Orders WHERE Name LIKE '%abc%';           -- 不走索引
SELECT * FROM Orders WHERE Total + 1 = 100;             -- 不走索引

-- SARGable 改写：可走索引
SELECT * FROM Orders
WHERE CreatedAt >= '2024-01-01' AND CreatedAt < '2025-01-01';  -- 范围查询，走索引

SELECT * FROM Orders WHERE Name LIKE 'abc%';            -- 前缀匹配，可走索引
SELECT * FROM Orders WHERE Total = 99;                  -- 常数比较，走索引
```

### 7.3 什么是执行计划？如何分析慢查询？

- 执行计划：SQL 引擎如何执行查询（扫描/查找、连接方式、预估行数）。
- 排查：`SET STATISTICS IO ON`、`SET STATISTICS TIME ON`、SSMS 图形计划、`sys.dm_exec_query_stats`。
- 关注：表扫描 vs 索引查找、预估行数偏差（统计信息过期）、隐式转换、内存授予。

### 7.4 事务的 ACID 是什么？

- **原子性（Atomicity）**：全做或全不做。
- **一致性（Consistency）**：事务前后数据满足约束。
- **隔离性（Isolation）**：并发事务互不干扰。
- **持久性（Durability）**：提交后持久化，不因故障丢失。

### 7.5 SQL Server 事务隔离级别有哪些？（高频）

| 隔离级别 | 脏读 | 不可重复读 | 幻读 | 说明 |
|---|---|---|---|---|
| READ UNCOMMITTED | 可能 | 可能 | 可能 | 无锁读，性能最高 |
| READ COMMITTED（默认） | 否 | 可能 | 可能 | 共享锁读完即放 |
| REPEATABLE READ | 否 | 否 | 可能 | 共享锁持有到事务结束 |
| SNAPSHOT | 否 | 否 | 否 | 行版本控制，读不阻塞写 |
| SERIALIZABLE | 否 | 否 | 否 | 范围锁，并发最低 |

- 隔离级别越高一致性越好、并发越低；`READ COMMITTED SNAPSHOT`（RCSI）是生产常用平衡方案。


**代码示例（SQL Server）：**

```sql
-- 设置当前会话隔离级别
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;      -- 默认：不可脏读，可不可重复读
SET TRANSACTION ISOLATION LEVEL SNAPSHOT;            -- 行版本控制，读不阻塞写
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;        -- 范围锁，最高隔离

-- 开启 READ COMMITTED SNAPSHOT（生产常用平衡方案，库级配置）
ALTER DATABASE MyDb SET READ_COMMITTED_SNAPSHOT ON;

-- 验证隔离级别
SELECT transaction_isolation_level FROM sys.dm_exec_sessions
WHERE session_id = @@SPID;

-- EF Core 中设置
-- await using var tx = await db.Database.BeginTransactionAsync(
--     System.Data.IsolationLevel.Snapshot);
```

### 7.6 脏读、不可重复读、幻读是什么？

- **脏读**：读到其他事务**未提交**的数据（可能回滚）。
- **不可重复读**：同一事务内两次读同一行，值被其他事务**修改**（UPDATE）。
- **幻读**：同一事务内两次范围查询，结果行数变化（其他事务 **INSERT** 了满足条件的行）。

### 7.7 乐观锁与悲观锁？

- 悲观锁：先锁后操作（`UPDLOCK`/`SELECT FOR UPDATE`），冲突少、并发低、易死锁。
- 乐观锁：不锁，更新时校验版本/条件（`WHERE version = ?`），冲突抛错重试；读多写少推荐。


**代码示例（SQL Server）：**

```sql
-- 悲观锁：先锁后操作（UPDLOCK + 行锁，事务结束才释放）
BEGIN TRAN;
SELECT * FROM Inventory WITH (UPDLOCK, ROWLOCK)
WHERE ProductId = 100;                  -- 锁住该行，防止并发修改
-- ...业务处理...
UPDATE Inventory SET Stock = Stock - 1 WHERE ProductId = 100;
COMMIT;                                 -- 提交后释放锁

-- 乐观锁：不锁，更新时校验版本（WHERE version = ?）
-- 表结构：ALTER TABLE Inventory ADD Version INT NOT NULL DEFAULT 0;

BEGIN TRAN;
DECLARE @v INT = (SELECT Version FROM Inventory WHERE ProductId = 100);
-- ...业务处理...
UPDATE Inventory
SET Stock = Stock - 1, Version = Version + 1
WHERE ProductId = 100 AND Version = @v;   -- 影响行数 0 => 已被并发修改，需重试
IF @@ROWCOUNT = 0
BEGIN
    ROLLBACK;                             -- 冲突，回滚重试
    THROW 50001, '并发冲突，请重试', 1;
END
COMMIT;
```

### 7.8 什么是死锁？如何减少死锁？

- 死锁：两个事务各自持有对方需要的资源，互相等待。
- 减少：一致的访问顺序；事务短小；索引减少锁范围；`READ COMMITTED`/RCSI；`SET DEADLOCK_PRIORITY`/重试。

### 7.9 什么是数据库规范化与反规范化？

- 规范化：消除冗余（1NF/2NF/3NF），数据一致、少异常，但查询需多表 JOIN。
- 反规范化：适当冗余（冗余列、汇总表），提升读性能，牺牲写一致性与维护成本。
- 权衡：OLTP 适度规范化 + 必要冗余；报表库可反规范化/宽表。

### 7.10 Redis 常用数据结构有哪些？适用场景？

- **String**：缓存、计数器（`INCR`）、分布式锁（`SET NX EX`）。
- **Hash**：对象字段存储（用户资料）。
- **List**：消息队列（LPUSH/BRPOP，简易队列）。
- **Set**：去重、交集/并集（共同好友、标签）。
- **Sorted Set（ZSet）**：排行榜、延时队列（score = 时间戳）。
- **Bitmap/Stream**：签到、事件流。


**代码示例（Redis 命令）：**

```bash
# String：缓存、计数器、分布式锁
SET user:1:name "张三"
GET user:1:name
INCR visit:count                     # 原子自增（计数器）
SET lock:order:1 "token" NX PX 30000 # 分布式锁（原子设置+过期）

# Hash：对象字段存储（用户资料）
HSET user:1 name "张三" age 18 city "武汉"
HGETALL user:1

# List：消息队列（简易）
LPUSH queue:mail "msg1"
BRPOP queue:mail 0                    # 阻塞弹出

# Set：去重、交集/并集（共同好友、标签）
SADD tags:1 "csharp" ".net"
SINTER tags:1 tags:2                  # 交集

# Sorted Set（ZSet）：排行榜、延时队列（score = 时间戳）
ZADD rank 100 "userA" 90 "userB"
ZREVRANGE rank 0 9 WITHSCORES         # 排行榜前10

# Stream：事件流（Kafka 风格）
XADD events * type "order" id 1
```

### 7.11 Redis 持久化：RDB 与 AOF？

- **RDB**：快照文件，恢复快、体积小；可能丢最后一次快照后的数据。
- **AOF**：追加写命令日志，数据更安全（每秒/每命令 fsync）；文件大、恢复慢；AOF 重写压缩。
- 生产：两者结合（RDB 做备份恢复 + AOF 保证实时性）。

### 7.12 缓存穿透、击穿、雪崩（Redis 视角）？（高频，见 5.22）

- 穿透：查不存在 key 打爆 DB → 缓存空值 + 布隆过滤器。
- 击穿：热点 key 过期 → 互斥锁重建 / 逻辑过期。
- 雪崩：大量 key 同时过期 / Redis 宕机 → TTL 随机化、多级缓存、限流降级、Redis 高可用。

### 7.13 如何实现分布式锁？有什么坑？（高频）

- 方案：Redis `SET key value NX PX 30000`（原子设置 + 过期）；释放时用 Lua 脚本校验 value 再 DEL（防误删他人锁）；Redisson/`RedLock` 续期看门狗。
- **坑**：业务未完成锁过期（续期）；释放别人的锁（value 校验）；主从切换锁丢失（RedLock/多节点）；本地锁（`lock`）不能跨进程。
- 备选：数据库行锁、ZooKeeper 顺序节点。


**代码示例（StackExchange.Redis）：**

```csharp
// 加锁：SET key value NX PX（原子设置 + 过期），value 用唯一标识
string token = Guid.NewGuid().ToString("N");
bool ok = await db.StringSetAsync(
    key, token, TimeSpan.FromSeconds(30), When.NotExists);
if (!ok) throw new Exception("获取锁失败");

try
{
    await DoBusinessAsync();            // 业务（若超长需看门狗续期）
}
finally
{
    // 释放：Lua 脚本校验 value 再 DEL，防止误删别人的锁
    const string script = """
        if redis.call("get", KEYS[1]) == ARGV[1]
        then return redis.call("del", KEYS[1])
        else return 0 end
        """;
    await db.ScriptEvaluateAsync(script, new RedisKey[] { key },
        new RedisValue[] { token });
}

// 坑：
// 1. 业务未完成锁过期 -> 看门狗自动续期（Redisson/自研）
// 2. 释放别人的锁 -> 必须 value 校验（上面的 Lua）
// 3. 主从切换锁丢失 -> RedLock 多节点 / ZooKeeper
// 4. 本地 lock 不能跨进程
```

### 7.14 什么是布隆过滤器（Bloom Filter）？

- 概率数据结构：判断"**一定不存在 / 可能存在**"，用多个哈希位图。
- 用途：缓存穿透前置过滤、判重、爬虫去重；不可删除元素（Counting Bloom Filter 可删）。
- 误判率与位数组大小、哈希次数相关。

### 7.15 什么是连接池（数据库）？

- 复用数据库连接，避免高频建连开销；`Max Pool Size`/`Min Pool Size`；连接字符串中 `Pooling=true`。
- 泄漏症状：超时异常（timeout expired）；用 `sp_who2`/`sys.dm_exec_sessions` 检查。

### 7.16 什么是 NoSQL？什么时候用？

- NoSQL：非关系型（文档 MongoDB、KV Redis、列族 Cassandra、图 Neo4j）。
- 选择：高并发 KV/缓存（Redis）、灵活文档模型（MongoDB）、海量写入扩展（Cassandra）、关系与事务要求强用关系库。
- 注意：强一致/复杂查询/事务场景 NoSQL 不占优，混合架构常见。

### 7.17 什么是分库分表？有哪些策略？

- 垂直分库（按业务拆）、水平分表（按 ID/时间哈希路由）。
- 路由：取模、范围、一致性哈希；全局 ID（雪花算法、号段）。
- 问题：跨库 JOIN/事务难（分而治之 + 冗余 + 最终一致）；迁移复杂（双写）。

### 7.18 什么是雪花算法（Snowflake）？

- 64 位 ID：时间戳 + 机器 ID + 序列号；趋势递增、分布式唯一。
- 变体：调整位数适应不同规模；时钟回拨处理。


**代码示例：**

```csharp
public class SnowflakeId
{
    private const long Twepoch = 1288834974657L;   // 起始时间戳
    private const int WorkerBits = 5;              // 机器 ID 位数
    private const int SeqBits = 12;                // 序列号位数
    private const long MaxSeq = (1L << SeqBits) - 1;

    private readonly long _workerId;
    private long _lastTimestamp = -1L;
    private long _seq;
    private readonly object _lock = new();

    public SnowflakeId(long workerId) => _workerId = workerId;

    public long NextId()
    {
        lock (_lock)
        {
            long ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (ts == _lastTimestamp)
            {
                _seq = (_seq + 1) & MaxSeq;        // 同一毫秒内序列号递增
                if (_seq == 0) while (ts <= _lastTimestamp) ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            else _seq = 0;

            _lastTimestamp = ts;
            // 64 位：时间戳(41) | 机器ID(10) | 序列号(12)
            return ((ts - Twepoch) << 22) | (_workerId << 12) | _seq;
        }
    }
}
// 特点：趋势递增、分布式唯一；变体可调整位数；需处理时钟回拨
```

### 7.19 什么是 `LIKE` 查询优化？

- `'xx%'` 前缀匹配可走索引；`'%xx%'` 全表扫描（可用全文索引/倒排）。
- 大数据量文本搜索建议全文索引或 Elasticsearch。

### 7.20 什么是事务的传播与隔离（EF/DB 层）？

- 隔离级别通过 `SET TRANSACTION ISOLATION LEVEL` 或 EF `UseTransaction`/`IsolationLevel` 设置。
- 传播（Propagation）：REQUIRED（默认，无事务则新建）、REQUIRES_NEW（新事务）、NESTED 等（.NET 中一般手动管理事务边界，不像 Spring 有注解传播）。

---

<a id="c8"></a>

## 八、设计模式与架构原则

### 8.1 SOLID 原则是什么？（必问）

**要点：**
- **S（单一职责，SRP）**：一个类只负责一件事，只有一个变更理由。
- **O（开闭，OCP）**：对扩展开放、对修改关闭（用继承/接口/策略扩展，不直接改已有类）。
- **L（里氏替换，LSP）**：子类必须能替换父类而不破坏程序行为。
- **I（接口隔离，ISP）**：客户端不应依赖不需要的接口，接口小而专。
- **D（依赖倒置，DIP）**：依赖抽象（接口）不依赖具体实现；高层模块不依赖低层模块。

**加分**：结合项目举例（如 Service 依赖仓储接口而非 DbContext 直接实现）。

### 8.2 常见设计模式分类？

- **创建型**：单例、工厂方法、抽象工厂、建造者、原型。
- **结构型**：适配器、装饰器、外观、代理、组合、桥接、享元。
- **行为型**：策略、观察者、模板方法、命令、状态、迭代器、责任链、中介者。
- 现代 .NET 视角：**DI 容器内建**了许多模式（单例、工厂、装饰器可用注册实现），面试要能说出"在 .NET 中怎么落地"。

### 8.3 单例模式（Singleton）？线程安全问题？（高频）

**要点：**
- 确保一个类只有一个实例并全局访问。
- 线程安全实现：
  1. **`Lazy<T>`**（推荐）：`private static readonly Lazy<AppConfig> _instance = new(() => new AppConfig());`
  2. 静态字段初始化（CLR 保证线程安全）：`private static readonly Singleton _instance = new();`
  3. 双重检查锁（`volatile` + lock，双 if）。
- **滥用问题**：全局状态、难测试、隐藏依赖——现代代码优先用 **DI 的 `AddSingleton`**，而不是手写静态单例。
- 注意：Singleton 内共享状态必须线程安全。


**代码示例：**

```csharp
// 推荐：Lazy<T>（线程安全，惰性创建，简单）
public sealed class AppConfig
{
    private static readonly Lazy<AppConfig> _instance =
        new(() => new AppConfig());
    public static AppConfig Instance => _instance.Value;
    private AppConfig() { }          // 私有构造函数，禁止外部 new
    public string ConnectionString { get; set; } = "";
}

// 方式2：静态字段初始化（CLR 保证线程安全，程序集加载即创建）
public sealed class Singleton2
{
    public static readonly Singleton2 Instance = new();
    private Singleton2() { }
}

// 方式3：双重检查锁（volatile + lock）
public sealed class Singleton3
{
    private static volatile Singleton3? _instance;
    private static readonly object Lock = new();
    public static Singleton3 Instance
    {
        get
        {
            if (_instance is null)                     // 第一次检查（无锁）
                lock (Lock)
                    if (_instance is null)             // 第二次检查（有锁）
                        _instance = new Singleton3();
            return _instance;
        }
    }
    private Singleton3() { }
}

// 现代实践：优先 DI 的 AddSingleton，而非手写静态单例（可测试、依赖显式）
```

### 8.4 工厂模式与抽象工厂？

- **工厂方法**：父类定义创建接口，子类决定实例化哪个类（一个产品族的一个产品）。
- **抽象工厂**：创建**一族相关对象**（如 UI 主题的按钮+输入框+对话框）。
- .NET 落地：DI 注册 + `Func<string, IService>`（keyed services，.NET 8+）；简单工厂就是一个 `Create` 静态方法/类。
- 好处：解耦创建逻辑、便于替换实现。


**代码示例：**

```csharp
// 简单工厂：一个 Create 方法根据条件返回不同实现
public interface IPayment { void Pay(decimal amount); }

public class Alipay : IPayment { public void Pay(decimal m) => Console.WriteLine($"支付宝 {m}"); }
public class WeChatPay : IPayment { public void Pay(decimal m) => Console.WriteLine($"微信 {m}"); }

public static class PaymentFactory
{
    public static IPayment Create(string type) => type switch
    {
        "alipay" => new Alipay(),
        "wechat" => new WeChatPay(),
        _ => throw new NotSupportedException()
    };
}

// 抽象工厂：创建一族相关对象（如 UI 主题的按钮+输入框）
public interface IButton { void Render(); }
public interface IInput { void Render(); }
public interface IUiFactory
{
    IButton CreateButton();
    IInput CreateInput();
}
public class DarkUiFactory : IUiFactory
{
    public IButton CreateButton() => new DarkButton();
    public IInput CreateInput() => new DarkInput();
}

// .NET 落地：DI 注册 + keyed services（.NET 8+）
// builder.Services.AddKeyedScoped<IPayment, Alipay>("alipay");
// var pay = sp.GetRequiredKeyedService<IPayment>("alipay");
class DarkButton : IButton { public void Render() { } }
class DarkInput : IInput { public void Render() { } }
```

### 8.5 策略模式（Strategy）？

- 定义一族算法，各自封装可互换，运行时选择。
- 例：支付策略（支付宝/微信/银行卡）、价格计算策略。
- .NET 落地：接口 + 多个实现 + DI 注册（`AddScoped<IPayment, Alipay>()` 或 keyed services）；比 `switch` 更好扩展（OCP）。


**代码示例：**

```csharp
// 定义一族算法，各自封装可互换，运行时选择
public interface IPricingStrategy { decimal Calc(decimal basePrice); }

public class NormalPricing : IPricingStrategy
{
    public decimal Calc(decimal p) => p;
}
public class DiscountPricing : IPricingStrategy
{
    private readonly decimal _rate;
    public DiscountPricing(decimal rate) => _rate = rate;
    public decimal Calc(decimal p) => p * _rate;
}
public class VipPricing : IPricingStrategy
{
    public decimal Calc(decimal p) => p * 0.8m;
}

// 上下文：组合策略，替换 switch（OCP：加策略不改已有代码）
public class OrderPricer
{
    private readonly IPricingStrategy _strategy;
    public OrderPricer(IPricingStrategy strategy) => _strategy = strategy;
    public decimal Price(decimal basePrice) => _strategy.Calc(basePrice);
}

// 使用
var pricer = new OrderPricer(new VipPricing());
Console.WriteLine(pricer.Price(100));   // 80

// .NET 落地：接口 + 多实现 + DI（keyed services 按类型取）
```

### 8.6 观察者模式（Observer）？与事件的关系？

- 一对多通知：被观察者状态变化时通知所有观察者。
- .NET 中**事件（event）就是观察者模式的实现**；还有 `IObservable<T>`/`IObserver<T>`（Rx）、`INotifyPropertyChanged`（WPF/MAUI 绑定）。
- 注意内存泄漏：订阅后忘记退订（事件源持有订阅者引用）。


**代码示例：**

```csharp
// .NET 中事件（event）就是观察者模式的实现
public class WeatherStation
{
    public event EventHandler<WeatherChangedEventArgs>? WeatherChanged;

    private string _weather = "晴";
    public string Weather
    {
        get => _weather;
        set
        {
            _weather = value;
            WeatherChanged?.Invoke(this, new WeatherChangedEventArgs(value));  // 通知所有订阅者
        }
    }
}

public class WeatherChangedEventArgs : EventArgs
{
    public string NewWeather { get; }
    public WeatherChangedEventArgs(string w) => NewWeather = w;
}

// 观察者（订阅者）
var station = new WeatherStation();
EventHandler<WeatherChangedEventArgs> display = (_, e) => Console.WriteLine($"显示：{e.NewWeather}");
EventHandler<WeatherChangedEventArgs> alert = (_, e) =>
{
    if (e.NewWeather == "暴雨") Console.WriteLine("警报！");
};
station.WeatherChanged += display;
station.WeatherChanged += alert;

station.Weather = "暴雨";   // 两个订阅者都被通知

// 注意：订阅后要退订（-=），否则事件源持有订阅者引用 -> 内存泄漏
station.WeatherChanged -= display;
```

### 8.7 装饰器模式（Decorator）？

- 动态给对象**添加职责**，包装原始对象，不修改原类。
- 例：日志装饰器、缓存装饰器、重试装饰器。
- .NET 落地：DI 支持装饰器（`TryAdd` + 手动包装，或 Scrutor）；`IEnumerable<T>` 注入可拿多个实现组合。
- 与继承对比：组合优于继承，更灵活。


**代码示例：**

```csharp
// 动态给对象添加职责，包装原始对象，不修改原类
public interface IReportService { string Generate(); }

public class ReportService : IReportService
{
    public string Generate() => "报表数据";
}

// 装饰器1：加缓存
public class CachedReportService : IReportService
{
    private readonly IReportService _inner;
    public CachedReportService(IReportService inner) => _inner = inner;

    public string Generate()
    {
        Console.WriteLine("命中缓存");
        return _inner.Generate();
    }
}

// 装饰器2：加日志
public class LoggedReportService : IReportService
{
    private readonly IReportService _inner;
    public LoggedReportService(IReportService inner) => _inner = inner;

    public string Generate()
    {
        Console.WriteLine("记录日志");
        return _inner.Generate();
    }
}

// 组合使用（组合优于继承）
IReportService svc = new LoggedReportService(new CachedReportService(new ReportService()));
svc.Generate();   // 记录日志 -> 命中缓存 -> 报表数据

// DI 落地：Scrutor 库自动装饰；IEnumerable<T> 注入可拿多个实现组合
```

### 8.8 适配器模式（Adapter）与外观模式（Facade）？

- **适配器**：把不兼容接口转换成客户端期望的接口（如旧系统 API 封装成新接口）。
- **外观**：为复杂子系统提供统一简化入口（如一个 `OrderFacade` 封装下单全流程）。
- 区别：适配器改变接口，外观简化接口；都用于解耦。


**代码示例：**

```csharp
// 适配器：把不兼容接口转换成客户端期望的接口
// 旧系统接口
public class LegacyPaymentApi
{
    public string DoPay(string reqXml) => "old result";
}

// 新系统期望的接口
public interface INewPayment { Task<bool> PayAsync(decimal amount); }

// 适配器：包装旧接口
public class LegacyPaymentAdapter : INewPayment
{
    private readonly LegacyPaymentApi _legacy = new();
    public Task<bool> PayAsync(decimal amount)
    {
        string xml = $"<amount>{amount}</amount>";       // 转换格式
        var result = _legacy.DoPay(xml);
        return Task.FromResult(result == "old result");
    }
}

// 外观：为复杂子系统提供统一简化入口
public class OrderFacade
{
    private readonly IInventory _inv;
    private readonly IPayment _pay;
    private readonly INotify _notify;
    public OrderFacade(IInventory inv, IPayment pay, INotify notify)
    { _inv = inv; _pay = pay; _notify = notify; }

    public void PlaceOrder(long productId, decimal price)
    {
        _inv.Deduct(productId);      // 内部编排多个子系统
        _pay.Charge(price);
        _notify.Send("下单成功");
    }
}
interface IInventory { void Deduct(long id); }
interface IPayment { void Charge(decimal m); }
interface INotify { void Send(string msg); }
// 区别：适配器改变接口，外观简化接口
```

### 8.9 建造者模式（Builder）？

- 分步骤构造复杂对象，同一构建过程可产生不同表示。
- 例：`StringBuilder`（经典）、`WebApplicationBuilder`、配置 `Host.CreateDefaultBuilder`、Fluent 配置。
- 好处：参数多、可选参数多的对象构造可读性好。


**代码示例：**

```csharp
// 分步骤构造复杂对象，同一构建过程可产生不同表示
public class Email
{
    public string? To { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public bool HasAttachment { get; set; }
}

public class EmailBuilder
{
    private readonly Email _email = new();

    public EmailBuilder To(string to) { _email.To = to; return this; }        // 返回 this 支持链式
    public EmailBuilder WithSubject(string s) { _email.Subject = s; return this; }
    public EmailBuilder WithBody(string b) { _email.Body = b; return this; }
    public EmailBuilder WithAttachment()
    {
        _email.HasAttachment = true;
        return this;
    }
    public Email Build() => _email;
}

// 使用：链式、可读性好
var email = new EmailBuilder()
    .To("a@x.com")
    .WithSubject("面试通知")
    .WithBody("恭喜通过初筛")
    .WithAttachment()
    .Build();

// .NET 经典例子：StringBuilder、WebApplicationBuilder、Host.CreateDefaultBuilder
```

### 8.10 责任链模式（Chain of Responsibility）？与中间件的关系？

- 请求沿链传递，每个处理器决定处理或传给下一个。
- **ASP.NET Core 中间件管道就是责任链模式的实现**；异常过滤器链、认证 Handler 也是。
- 好处：解耦发送者与接收者、灵活增减处理环节。


**代码示例：**

```csharp
// 请求沿链传递，每个处理器决定处理或传给下一个
public abstract class Handler
{
    private Handler? _next;
    public Handler SetNext(Handler next) { _next = next; return next; }

    public void Handle(string request)
    {
        if (CanHandle(request))
            Process(request);
        else if (_next is not null)
            _next.Handle(request);      // 传给下一个
        else
            Console.WriteLine("无人处理");
    }

    protected abstract bool CanHandle(string request);
    protected abstract void Process(string request);
}

public class AuthHandler : Handler
{
    protected override bool CanHandle(string r) => r.StartsWith("auth:");
    protected override void Process(string r) => Console.WriteLine($"认证处理 {r}");
}
public class LogHandler : Handler
{
    protected override bool CanHandle(string r) => r.StartsWith("log:");
    protected override void Process(string r) => Console.WriteLine($"日志处理 {r}");
}

// 组装链
var chain = new AuthHandler();
chain.SetNext(new LogHandler());
chain.Handle("log:hello");     // 认证跳过 -> 日志处理
chain.Handle("auth:token");    // 认证处理

// ASP.NET Core 中间件管道就是责任链模式的实现（Use/next/短路）
```

### 8.11 模板方法模式（Template Method）？

- 父类定义算法骨架，子类实现可变步骤。
- 例：`BackgroundService.ExecuteAsync`（骨架）+ 子类实现具体循环逻辑；`Stream`、`FileSystemWatcher` 等。
- 与策略的区别：模板方法是继承式（骨架在父类），策略是组合式（算法可替换）。


**代码示例：**

```csharp
// 父类定义算法骨架，子类实现可变步骤
public abstract class DataImporter
{
    // 模板方法：固定骨架，子类不能改变流程
    public void Import(string file)
    {
        Validate(file);           // 骨架步骤
        var data = Parse(file);   // 抽象步骤：子类实现
        Save(data);
        Notify();
    }

    private void Validate(string f) => Console.WriteLine($"校验 {f}");
    protected abstract List<string> Parse(string file);   // 子类实现
    private void Save(List<string> data) => Console.WriteLine($"保存 {data.Count} 条");
    protected virtual void Notify() { }                   // 可选钩子
}

public class CsvImporter : DataImporter
{
    protected override List<string> Parse(string file) => file.Split('\n').ToList();
}

// 使用
new CsvImporter().Import("a.csv");   // 校验 -> 解析(Csv) -> 保存

// .NET 例子：BackgroundService.ExecuteAsync（骨架）+ 子类实现循环逻辑
// 与策略的区别：模板方法继承式（骨架在父类），策略组合式（算法可替换）
```

### 8.12 什么是仓储模式（Repository）？.NET 中是否必要？

- 仓储：把数据访问抽象为集合式接口（`IOrderRepository`），业务层不直接依赖 EF。
- **争议**：EF Core 本身已是仓储 + 工作单元（DbContext = UoW），简单 CRUD 再加一层仓储是冗余（微软官方建议除非有明确收益）。
- 何时值得用：多数据源切换、复杂查询域逻辑集中、单元测试隔离需求强。

### 8.13 什么是选项模式（Options Pattern）？（见 5.6）

- 强类型配置绑定 + 注入；`IConfiguration` → `IOptions<T>`；支持验证与监听变更。

### 8.14 什么是 `Facade` 与 `Mediator` 的区别？

- Facade：统一入口（结构性）。
- Mediator（中介者）：对象间交互通过中介协调，避免网状依赖（行为型）；事件总线、`IMediator`（MediatR 库）是常见实现。
- MediatR 在 .NET 中广泛用于 CQRS/事件驱动：`IRequest`/`INotification` + Handler。


**代码示例：**

```csharp
// Mediator（中介者）：对象间交互通过中介协调，避免网状依赖
// .NET 常用实现：MediatR 库（CQRS/事件驱动）

// 1. 定义请求
public record CreateOrderCommand(long ProductId, int Qty) : IRequest<long>;

// 2. 定义 Handler（处理者）
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, long>
{
    private readonly AppDbContext _db;
    public CreateOrderHandler(AppDbContext db) => _db = db;

    public async Task<long> Handle(CreateOrderCommand cmd, CancellationToken ct)
    {
        var order = new Order { ProductId = cmd.ProductId, Qty = cmd.Qty };
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);
        return order.Id;
    }
}

// 3. 发布事件（领域事件）
public record OrderCreated(long OrderId) : INotification;
public class OrderCreatedHandler : INotificationHandler<OrderCreated>
{
    public Task Handle(OrderCreated e, CancellationToken ct)
    {
        Console.WriteLine($"发送通知：订单 {e.OrderId} 创建");
        return Task.CompletedTask;
    }
}

// 4. 使用：通过 IMediator 发送，发送者与处理者完全解耦
// await mediator.Send(new CreateOrderCommand(1, 2));
// await mediator.Publish(new OrderCreated(1));

// 区别：Facade 是结构性统一入口；Mediator 是行为型协调交互
```

### 8.15 什么是 DDD（领域驱动设计）核心概念？（高级）

- 限界上下文（Bounded Context）、聚合（Aggregate）、实体/值对象、仓储、领域事件、应用服务与领域服务。
- 战略设计（上下文映射）与战术设计（聚合建模）。
- .NET 落地：分层（Domain/Application/Infrastructure/API）、MediatR、EF Core 映射聚合。
- 面试关注：什么时候用 DDD（业务复杂）、不要为了 DDD 而 DDD。

---

<a id="c9"></a>

## 九、微服务与分布式系统

### 9.1 什么是微服务？优缺点是什么？（必问）

**要点：**
- 微服务：把应用拆分为一组**独立部署、独立扩展、独立技术栈**的小服务，通过轻量协议通信（HTTP/gRPC/消息）。
- **优点**：独立部署与扩展、故障隔离、技术异构、团队自治（康威定律）、可伸缩性好。
- **缺点**：分布式复杂度高（网络、一致性、调试）、运维成本大、数据一致性难、性能开销（跨服务调用）、测试复杂。
- **何时不要用**：小团队/简单业务用单体（Modular Monolith）更合适；微服务是权衡不是银弹。

### 9.2 服务间通信方式有哪些？

- **同步**：REST（HTTP/JSON）、gRPC（HTTP/2 + Protobuf）。
- **异步**：消息队列（RabbitMQ、Kafka、Azure Service Bus）、事件流。
- 选择：强实时响应走同步；解耦、削峰、最终一致走消息。
- 协议细节：REST 简单通用；gRPC 性能高、契约强、双向流。

### 9.3 为什么用消息队列？常用产品对比？（高频）

- 好处：**解耦**（生产/消费独立演进）、**削峰填谷**（缓冲突发流量）、**异步化**（慢任务移出请求路径）、**失败隔离**（消费者宕机消息不丢）、**水平扩展**（并行消费）。
- 对比：
  - **RabbitMQ**：功能全（路由、确认、死信）、延迟低、易用；吞吐中等。
  - **Kafka**：高吞吐、日志/事件流场景强、分区有序、持久化好；运维较重。
  - **RocketMQ**：阿里系、事务消息、延迟消息友好。
- .NET 客户端：MassTransit（RabbitMQ/Azure Service Bus/SQS 抽象，重试、Saga、Outbox）、NServiceBus（商业）、原生 SDK。

### 9.4 如何保证消息不丢失？如何保证不重复消费？（高频）

- **不丢失**（生产者→Broker→消费者三段）：
  - 生产者：确认机制（RabbitMQ `publisher confirm`；Kafka `acks=all`）；
  - Broker：持久化（RabbitMQ durable queue + persistent message；Kafka `replication.factor`）；
  - 消费者：**手动确认**（`basic.ack`/commit offset），处理成功再确认；失败进**死信队列**人工处理。
- **不重复（幂等消费）**：
  - 消息携带唯一 ID，消费端按 ID 去重（DB 唯一约束/Redis 记录）；
  - 业务操作本身幂等（如状态机、版本号）；
  - 至少一次投递 + 幂等消费是标准组合（恰好一次代价高）。


**代码示例（RabbitMQ 消费者手动确认）：**

```csharp
// 消费者手动确认：处理成功再 ack，失败进死信队列（不丢失）
using var channel = connection.CreateChannel();
channel.BasicQos(0, 1, false);    // 一次拉取一条，处理完再取下一条

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.Received += async (_, ea) =>
{
    try
    {
        var msg = System.Text.Json.JsonSerializer
            .Deserialize<OrderMsg>(ea.Body.Span);
        await ProcessAsync(msg!);          // 业务处理

        channel.BasicAck(ea.DeliveryTag, false);   // 确认：成功才 ack
    }
    catch (Exception ex)
    {
        // 失败：进死信队列（DLX）人工处理，而不是直接丢弃
        channel.BasicNack(ea.DeliveryTag, false, requeue: false);
        Console.WriteLine($"处理失败，进死信队列: {ex.Message}");
    }
};
channel.BasicConsume(queue: "orders", autoAck: false, consumer: consumer);

// 不重复消费（幂等）：消息带唯一 ID，消费端按 ID 去重
// 1. DB 唯一约束：INSERT ... ON CONFLICT / 唯一索引（重复插入抛异常 -> 视为已消费）
// 2. Redis SETNX msgId 记录已处理
// 3. 业务本身幂等（状态机：仅允许 Pending -> Paid 转移）

// 生产者侧：publisher confirm；Broker 侧：durable queue + persistent message
record OrderMsg(long Id, decimal Amount);
static Task ProcessAsync(OrderMsg m) => Task.CompletedTask;
```

### 9.5 什么是 Outbox 模式？（高级）

- 解决"**本地事务提交与发消息不一致**"：把业务变更和 outbox 事件记录放在**同一数据库事务**；后台 Relay（轮询/CDC）读 outbox 表发消息，成功后标记已发送。
- 好处：不依赖分布式事务，保证最终一致；MassTransit、Npgsql/EF 有现成实现。


**代码示例（核心流程）：**

```csharp
// 问题：本地事务提交与发消息不一致（DB 提交成功但消息没发出去）
// Outbox 方案：业务变更 + outbox 事件记录放同一个数据库事务

// 1. 业务操作和事件记录在同一事务
await using var tx = await db.Database.BeginTransactionAsync();

db.Orders.Add(new Order { ProductId = 1, Qty = 2 });
db.OutboxMessages.Add(new OutboxMessage
{
    Id = Guid.NewGuid(),
    Type = "OrderCreated",
    Payload = System.Text.Json.JsonSerializer.Serialize(new { OrderId = 1L }),
    CreatedAt = DateTime.UtcNow,
    Processed = false
});
await db.SaveChangesAsync();
await tx.CommitAsync();      // 一起提交：要么都有，要么都没有

// 2. 后台 Relay（轮询或 CDC）读取 outbox 表发消息
public class OutboxRelay : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var pending = await db.OutboxMessages
                .Where(m => !m.Processed)
                .OrderBy(m => m.CreatedAt)
                .Take(100)
                .ToListAsync(ct);

            foreach (var msg in pending)
            {
                await bus.PublishAsync(msg.Type, msg.Payload);
                msg.Processed = true;      // 发送成功后标记（失败可重试）
            }
            await db.SaveChangesAsync(ct);
            await Task.Delay(1000, ct);
        }
    }
}

// 好处：不依赖分布式事务，保证最终一致；MassTransit/EF 有现成实现
class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = "";
    public string Payload { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public bool Processed { get; set; }
}
```

### 9.6 什么是分布式事务？Saga 模式是什么？（高频）

**要点：**
- 分布式事务：跨多个服务/数据库的事务一致性；2PC 强一致但阻塞、实现复杂。
- **Saga**：把长事务拆成一系列本地事务 + **补偿（Compensation）**；任一失败则按逆序执行补偿操作，保证**最终一致**。
  - 编排式（Orchestration）：中心协调器指挥各参与方；
  -  choreography（编排）：各服务通过事件自发联动。
- 适用：订单-支付-库存-物流链路；业务上可接受短暂不一致。
- 还有：本地消息表、事务消息（RocketMQ）、TCC（Try-Confirm-Cancel，预留资源）。

### 9.7 什么是服务发现？.NET 中怎么做？

- 服务动态注册与发现（实例 IP/端口会变化，不能硬编码）。
- 实现：Consul、Etcd、ZooKeeper、K8s（Service/DNS）。
- .NET：`Steeltoe`（Consul/Eureka）、`Microsoft.Extensions.ServiceDiscovery`（.NET 8+，配合 YARP 网关）。

### 9.8 什么是 API 网关？常见功能？

- 网关是微服务的统一入口：路由转发、负载均衡、认证鉴权、限流、熔断、协议转换、聚合。
- 常见：Ocelot（.NET 老牌）、YARP（微软反向代理库，可编程）、Kong、APISIX、云厂商网关。
- 注意：网关也是单点，需高可用；避免把业务逻辑塞进网关。

### 9.9 什么是熔断、降级、限流？（高频）

- **熔断（Circuit Breaker）**：下游故障时快速失败，避免雪崩（Polly `CircuitBreakerPolicy`；状态：关闭→打开→半开）。
- **降级（Fallback）**：服务不可用时返回兜底结果（缓存数据、默认值、提示）。
- **限流（Rate Limit）**：保护自身不被过量请求打垮（令牌桶/固定窗口）。
- .NET 落地：Polly（重试、熔断、超时、降级）+ `IHttpClientFactory` 组合；内置 `AddRateLimiter`。


**代码示例（Polly + IHttpClientFactory）：**

```csharp
// Polly v8 弹性策略：重试 + 熔断 + 超时 + 降级
builder.Services.AddHttpClient("OrderApi", c =>
{
    c.BaseAddress = new Uri("https://order-svc");
})
.AddResilienceHandler("default", builder =>
{
    // 重试：指数退避 + 抖动
    builder.AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromMilliseconds(200),
        BackoffType = DelayBackoffType.Exponential
    });

    // 超时
    builder.AddTimeout(TimeSpan.FromSeconds(5));

    // 熔断：连续 5 次失败打开 30 秒（快速失败，避免雪崩）
    builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        FailureRatio = 0.5,
        SamplingDuration = TimeSpan.FromSeconds(30),
        MinimumThroughput = 5,
        BreakDuration = TimeSpan.FromSeconds(30)
    });
});

// 降级（Fallback）：服务不可用时返回兜底结果
// var response = await CallOrderApiAsync()
//     .ConfigureAwait(...)
//     .FallbackAsync(_ => Task.FromResult(new OrderDto { Status = "降级数据" }));
// 或 try/catch 捕获 Polly 抛出的 BrokenCircuitException 返回缓存

// 限流：见 5.32（AddRateLimiter）；网关层 + 应用层双层
// 注意重试陷阱：非幂等请求重试会造成重复扣款（配合幂等键）
```

### 9.10 什么是 CQRS？什么是事件溯源（Event Sourcing）？（高级）

- **CQRS**：命令（写）与查询（读）分离，读写模型独立扩展、独立优化（读用投影/物化视图）。
- 好处：读写负载差异大时解耦扩展；配合消息实现最终一致。
- **事件溯源**：不存当前状态，存**状态变更事件序列**，状态由事件重放得到；审计完整、可回溯；实现复杂（快照、版本演进）。
- .NET：MediatR + EF Core（写） + 查询投影表（读）；EventStoreDB。

### 9.11 什么是 Docker？镜像与容器？

- 镜像：只读模板（分层文件系统）；容器：镜像的运行实例（隔离进程）。
- 一致性（开发/测试/生产一致）、轻量（共享内核，比 VM 资源占用小）、快速启动。
- Dockerfile 常用指令：FROM、WORKDIR、COPY、RUN、EXPOSE、ENTRYPOINT。
- .NET 官方镜像：`mcr.microsoft.com/dotnet/sdk`（构建）、`aspnet`（运行时）。

### 9.12 什么是 Kubernetes（K8s）？核心概念？

- 容器编排平台：自动部署、扩缩容、自愈、服务发现、滚动更新。
- 核心：Pod（最小调度单元）、Deployment（无状态应用）、Service（稳定访问入口）、ConfigMap/Secret（配置）、Ingress（入口）、HPA（自动扩缩容）、liveness/readiness 探针。

### 9.13 什么是链路追踪（Distributed Tracing）？

- 跨服务追踪一次请求的完整调用链：trace ID + span（父子关系）。
- 标准：OpenTelemetry（标准库，可接 Jaeger、Zipkin、SkyWalking、APM）。
- .NET：`AddOpenTelemetry()` + ActivitySource；配合结构化日志关联 traceId 排查问题。


**代码示例（OpenTelemetry）：**

```csharp
// 标准：OpenTelemetry（可接 Jaeger、Zipkin、SkyWalking、APM）
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()      // 自动记录 HTTP 请求 Span
               .AddHttpClientInstrumentation()      // 自动记录 HttpClient 调用
               .AddSqlClientInstrumentation()       // 自动记录 SQL
               .AddSource("MyApp");
    })
    .WithMetrics(m => m.AddAspNetCoreInstrumentation())
    .UseOtlpExporter(o => o.Endpoint = new Uri("http://jaeger:4317"));

// 自定义 Span（手动埋点）
using var activity = MyActivitySource.StartActivity("CreateOrder");
activity?.SetTag("order.id", orderId);
activity?.SetTag("user.id", userId);

// 跨服务传播：trace ID + span ID 通过 HTTP 头（traceparent）自动传递
// 结构化日志中关联 traceId：
// builder.Services.AddOpenTelemetry().WithTracing(...);
// Serilog：.Enrich.WithTraceIdentifier()
static readonly System.Diagnostics.ActivitySource MyActivitySource =
    new("MyApp");
```

### 9.14 什么是配置中心？为什么需要？

- 集中管理多服务配置、支持动态刷新、版本管理：Consul KV、Apollo、Nacos、Azure App Configuration。
- 与本地 appsettings 对比：环境多、实例多时避免逐个改配置；敏感配置集中管理（Secret 加密）。

### 9.15 什么是幂等与最终一致性（结合微服务）？

- 见 9.4/9.6：消息至少一次 + 幂等消费；Saga + 补偿；Outbox + 重试；最终一致是微服务常态。
- 面试要能设计一个"下单→扣库存→支付"的最终一致方案。

### 9.16 什么是 gRPC 流式通信？（高级）

- 一元调用、服务端流、客户端流、双向流（HTTP/2）。
- 场景：实时数据推送（监控指标）、大文件分片、聊天。

### 9.17 什么是 Strangler Fig（绞杀者）模式？

- 渐进式改造：旧系统边缘加新服务，逐步替换功能，最后绞杀掉旧系统；降低一次性重写风险。

### 9.18 什么是弹性设计（Resilience）？.NET 中的实践？

- 超时、重试（指数退避 + 抖动）、熔断、舱壁隔离（Bulkhead，线程/信号量隔离）、降级。
- .NET 8+：**`Microsoft.Extensions.Resilience`**（内建重试/超时/熔断/舱壁中间件），或 Polly v8。
- 注意重试陷阱：非幂等请求重试会造成重复扣款（配合幂等键）。


**代码示例（.NET 8+ Resilience / Polly v8）：**

```csharp
// 核心策略：超时、重试（指数退避+抖动）、熔断、舱壁隔离、降级
builder.Services.AddHttpClient("PaymentSvc", c =>
{
    c.BaseAddress = new Uri("https://payment-svc");
})
.AddResilienceHandler("payment-pipeline", r =>
{
    // 舱壁隔离：最多 10 个并发调用，避免下游慢拖垮整个应用
    r.AddConcurrencyLimiter(10, 100);

    // 超时
    r.AddTimeout(TimeSpan.FromSeconds(3));

    // 重试：2 次，指数退避
    r.AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 2,
        BackoffType = DelayBackoffType.Exponential,
        Delay = TimeSpan.FromMilliseconds(300)
    });

    // 熔断：失败率 50% 且最小吞吐 5 时打开 20 秒
    r.AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        FailureRatio = 0.5,
        MinimumThroughput = 5,
        BreakDuration = TimeSpan.FromSeconds(20)
    });
});

// 使用
var client = httpFactory.CreateClient("PaymentSvc");
try
{
    var resp = await client.PostAsJsonAsync("/pay", request);
}
catch (BrokenCircuitException)      // 熔断打开：快速失败
{
    return CachedResult;            // 降级：返回兜底数据
}

// 注意重试陷阱：非幂等请求重试会造成重复扣款（配合幂等键）
```

### 9.19 什么是 Sidecar 与服务网格？（高级）

- Sidecar：与主服务同 Pod 的辅助容器（日志、代理、监控）。
- 服务网格（Service Mesh）：统一管理服务间通信（Istio、Linkerd）：流量管理、mTLS、可观测性；业务代码无侵入。

### 9.20 单体、模块化单体、微服务的取舍？

- **单体**：简单、快、事务容易；缺点是扩展粒度粗、部署耦合。
- **模块化单体**：代码分层分模块（Modular Monolith），部署仍单体，先模块化再拆微服务的中间路线（推荐演进路径）。
- **微服务**：组织规模与流量足够大、团队可自治时才拆；从模块化单体按边界逐步拆分。

---

<a id="c10"></a>

## 十、性能优化与内存管理

### 10.1 .NET 会有内存泄漏吗？常见原因有哪些？（必问）

**要点：**
- 有 GC 但**仍会泄漏**（对象仍可达，GC 无法回收）：
  1. **事件订阅未退订**：长期对象持有短期对象（发布者事件引用订阅者）→ 订阅者永不回收；
  2. **静态集合/静态缓存无限增长**：`static Dictionary` 无边界/无过期；
  3. **未释放非托管资源**：`HttpClient`、`DbConnection`、文件流、`SemaphoreSlim` 等未 Dispose；
  4. **async 闭包捕获大对象**；
  5. **未注销的定时器/回调**（`Timer`、`CancellationTokenSource` 注册回调）；
  6. **本机内存（Native）泄漏**：P/Invoke 未释放。
- 解决：WeakEvent 模式、`MemoryCache`（有过期策略）、`using`/`IAsyncDisposable`、退订事件。

### 10.2 如何排查 .NET 内存问题？（高级）

**要点：**
- 诊断工具链（dotnet 全局工具）：
  - `dotnet-counters`：实时看 GC 堆大小、`gc.gen0/1/2` 次数、线程数；
  - `dotnet-dump`：抓进程 dump；
  - `dotnet-gcdump`：抓堆快照分析对象分布（**关键**：比较两个时间点快照，找出持续增长的 Type）；
  - `dotnet-trace`：性能剖析（CPU）；
  - `dotnet-stack`：线程栈。
- 关键指标：`dotnet.gc.last_collection.heap.size`（存活堆是否持续增长）、`working set`（不要只看已提交内存——.NET 不会主动归还 OS，需结合存活对象判断）。
- 常用步骤：连抓两个 gcdump → 对比增长对象 → 定位根引用（静态/事件/缓存）。

### 10.3 什么是 BenchmarkDotNet？如何做基准测试？

- .NET 标准基准库：自动 warmup、多次运行、统计校验（防 JIT/噪声干扰）、内存分配报告（`[MemoryDiagnoser]`）。
- 用法：`[Benchmark]` 方法 + `BenchmarkRunner.Run<T>()`。
- 注意：不要用 `Stopwatch` 手动粗略计时（结论不可靠）；先对比方案再优化。


**代码示例：**

```csharp
// 安装：dotnet add package BenchmarkDotNet
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Text;

[MemoryDiagnoser]                    // 报告内存分配
public class StringBenchmark
{
    private readonly string[] _words = Enumerable.Range(0, 100)
        .Select(i => $"word{i}").ToArray();

    [Benchmark(Baseline = true)]     // 基准方法
    public string StringBuilder()
    {
        var sb = new StringBuilder();
        foreach (var w in _words) sb.Append(w);
        return sb.ToString();
    }

    [Benchmark]
    public string Concat()            // 对比方法
    {
        var s = "";
        foreach (var w in _words) s += w;
        return s;
    }
}

// 入口
// static void Main() => BenchmarkRunner.Run<StringBenchmark>();

// 输出示例：Mean、Allocated 对比——自动 warmup、多次运行、统计校验
// 不要用 Stopwatch 手动粗略计时（JIT/噪声干扰，结论不可靠）
```

### 10.4 `Span<T>` / `ArrayPool<T>` 如何提升性能？（高级）

- `Span<T>`：零分配切片、栈上分配，避免中间字符串/数组拷贝（字符串解析、协议解析）。
- `ArrayPool<T>.Shared`：数组池化复用，避免高频大数组分配（ASP.NET Core 内部大量使用）。
- 注意：`ArrayPool` 借出的数组**必须归还**且不能长期持有；用 `Memory<T>`/`IMemoryOwner` 管理。


**代码示例：**

```csharp
// Span：零分配切片，避免中间字符串/数组拷贝（协议/字符串解析）
string line = "192.168.1.1:8080";
ReadOnlySpan<char> span = line;
int colon = span.IndexOf(':');
ReadOnlySpan<char> ip = span[..colon];       // 切片无分配
ReadOnlySpan<char> port = span[(colon + 1)..];
Console.WriteLine(ip.ToString());            // 仅需要时再转 string

// ArrayPool：数组池化复用，避免高频大数组分配
byte[] buffer = ArrayPool<byte>.Shared.Rent(1024);   // 借出（可能比请求大）
try
{
    int read = stream.Read(buffer);
    Process(buffer.AsSpan(0, read));
}
finally
{
    ArrayPool<byte>.Shared.Return(buffer);   // 必须归还！不能长期持有
}

// 更安全：Memory<T> / IMemoryOwner 管理（可异步）
// 注意：借出的数组不要长期持有/二次归还
static void Process(Span<byte> s) { }
```

### 10.5 字符串拼接性能优化？

- 循环拼接用 `StringBuilder`；
- 高频插值用 `DefaultInterpolatedStringHandler`（编译器已优化）或 `String.Create`/`string.Concat` 精准合并；
- 避免 `+` 大循环、避免 `Format` 滥用；
- 用 `ReadOnlySpan<char>` 切片避免 `Substring` 分配。


**代码示例：**

```csharp
// 1. 循环拼接用 StringBuilder（避免 + 大循环产生大量垃圾对象）
var sb = new StringBuilder(1024);     // 预估容量减少扩容
foreach (var item in items) sb.Append(item);

// 2. 一次性拼接用插值（编译器优化为 DefaultInterpolatedStringHandler）
string s = $"Id={id}, Name={name}";

// 3. 高频场景可用 String.Create / string.Concat 精准合并
string joined = string.Concat("a", "b", "c");

// 4. 用 ReadOnlySpan 切片避免 Substring 分配
ReadOnlySpan<char> slice = full.AsSpan(0, 10);   // 无分配
string sub = slice.ToString();                   // 需要时才分配

// 5. 避免 string.Format 滥用（装箱 + 模板解析）
```

### 10.6 `HttpClient` 的正确使用？（高频）

- **不要**每次 `new HttpClient()`（Socket 耗尽风险：TIME_WAIT 堆积）；**不要**长期复用而不管生命周期（DNS 变更不生效）。
- 用 **`IHttpClientFactory`**：自动管理 handler 生命周期（默认 2 分钟轮换）、命名/类型化客户端、集成 Polly 策略。
- 现代（.NET 8+）：`HttpClient` + `SocketsHttpHandler`（`PooledConnectionLifetime`）手动管理也可行。
- 注意：复用同一个 `HttpClient` 是线程安全的；BaseAddress + 相对路径；`Dispose` 由工厂管理。


**代码示例：**

```csharp
// 错误：每次 new HttpClient() -> Socket 耗尽（TIME_WAIT 堆积）
// using var c = new HttpClient();   // 不要这样做

// 错误：长期复用静态 HttpClient 而不管生命周期 -> DNS 变更不生效

// 正确：IHttpClientFactory（自动管理 handler 生命周期，默认 2 分钟轮换）
builder.Services.AddHttpClient("Api", c =>
{
    c.BaseAddress = new Uri("https://api.example.com");
    c.Timeout = TimeSpan.FromSeconds(30);
});

public class ApiService
{
    private readonly IHttpClientFactory _factory;
    public ApiService(IHttpClientFactory factory) => _factory = factory;

    public async Task<string> GetUserAsync(long id)
    {
        var client = _factory.CreateClient("Api");   // 复用连接池
        return await client.GetStringAsync($"/users/{id}");
    }
}

// .NET 8+ 备选：HttpClient + SocketsHttpHandler 手动管理
// var handler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) };
// var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.example.com") };
// 注意：复用同一个 HttpClient 是线程安全的；Dispose 由工厂管理
```

### 10.7 异步代码常见性能坑？

- **sync-over-async**（`.Result`/`.Wait()`）：阻塞线程池线程 → 线程池饥饿 → 吞吐崩溃。
- **async void**：异常崩溃。
- 不必要 async：纯 CPU 短操作加 async 只是开销。
- 过度 `Task.Run`：把轻量同步代码丢线程池反而浪费。
- 并发 I/O 无节制：打满数据库/外部连接池（用 `SemaphoreSlim` 控制并发）。
- 每次都 `ConfigureAwait(false)` 不需要的场景过度使用。


**代码示例：**

```csharp
// 坑1：sync-over-async（.Result/.Wait()）阻塞线程池线程 -> 饥饿 -> 吞吐崩溃
// var r = GetAsync().Result;          // 禁止

// 坑2：async void -> 异常崩溃进程
// async void Bad() { throw new Exception(); }    // 仅事件处理器可用

// 坑3：不必要 async（纯 CPU 短操作）
public async Task<int> CalcAsync(int x)          // 无 await，白加状态机开销
{
    return x * 2;
}

// 坑4：过度 Task.Run（把轻量同步代码丢线程池反而浪费）
// await Task.Run(() => list.Where(...));        // 不如直接同步执行

// 坑5：并发 I/O 无节制 -> 打满数据库/外部连接池
// 用 SemaphoreSlim 控制并发
var sem = new SemaphoreSlim(20);
var tasks = urls.Select(async url =>
{
    await sem.WaitAsync();
    try { return await client.GetStringAsync(url); }
    finally { sem.Release(); }
});
var results = await Task.WhenAll(tasks);
```

### 10.8 什么是线程池饥饿（ThreadPool Starvation）？如何识别与解决？（高级）

- 现象：ThreadPool 线程全被阻塞（sync-over-async、长阻塞 I/O、`Thread.Sleep`、死锁），新任务排队，**请求大量超时**、吞吐骤降。
- 识别：`ThreadPool.ThreadCount` 持续接近最大、`pending work item count` 增长、GC 正常但延迟升高。
- 解决：消除阻塞（改真异步）、`ThreadPool.SetMinThreads` 应急、排查死锁、减少锁竞争。

### 10.9 什么是 GC 调优？Server GC 与 Workstation GC？

- Server GC：每核一个堆，多线程并行回收，吞吐高、内存多（**ASP.NET Core 默认**）。
- Workstation GC：单堆，低延迟友好，适合桌面端。
- 其他：`GCSettings.LatencyMode`（Batch/Interactive/LowLatency）、`GC.TryStartNoGCRegion`（限制性）、`DOTNET_gcServer` 等环境变量。
- 一般不要手动 `GC.Collect()`（除非测试/特殊场景）。

### 10.10 什么是 JIT 分层编译与 ReadyToRun？

- 分层编译：方法先快速编译（Tier 0），热点再优化编译（Tier 1），平衡启动与峰值。
- ReadyToRun（R2R）：发布时预编译部分代码，加快启动；动态功能受限部分仍走 JIT。
- Native AOT：全提前编译，启动最快但兼容性受限。

### 10.11 数据库查询性能优化清单？（高频）

1. 索引是否命中（SARGable、覆盖索引）；
2. 避免 N+1（Include/投影/拆分查询）；
3. 分页用键集分页替代 Skip/Take 大偏移；
4. 只取需要的列（投影）；
5. 批处理（ExecuteUpdate/Delete、批量插入库）；
6. 统计信息/执行计划检查；
7. 连接池、异步 API 防阻塞；
8. 热点数据缓存（Redis/内存）。

### 10.12 什么是内存屏障与伪共享？（高级）

- 内存屏障：保证指令顺序与可见性的 CPU 指令（`lock`/`volatile`/`Interlocked` 内部使用）。
- 伪共享：不同线程频繁访问同一缓存行（64 字节）的不同变量，互相失效缓存导致性能下降；用填充对齐避免（`[StructLayout]`/padding）。

### 10.13 什么是预热（Warmup）？如何减少冷启动影响？

- JIT 首次编译、静态初始化、连接建立等造成首次请求慢。
- 方案：预编译（AOT/R2R）、`dotnet watch` 不相关；ASP.NET Core 启动时预热（触发健康检查、预热缓存、`IHostedService` 预加载）、容器滚动更新避免全部实例同时冷启动。

### 10.14 如何做高并发写入优化？（场景）

- 批处理合并写、异步削峰（队列）、Redis 计数 + 定时落库、分库分表、乐观锁控制冲突、避免长事务与锁范围过大。

---

<a id="c11"></a>

## 十一、测试与质量保障

### 11.1 单元测试与集成测试的区别？

- **单元测试**：测试**单个方法/类**，隔离依赖（Mock），全内存、毫秒级、可并行、不碰 DB/网络/文件。
- **集成测试**：验证模块间协作（真实 DB、外部 API、整个管道），更接近真实但慢、易碎。
- 测试金字塔：多单测、少集成、更少 E2E。

### 11.2 xUnit、NUnit、MSTest 的区别？

- **xUnit**：现代主流，`[Fact]`/`[Theory]`，扩展性好（`IClassFixture`、`IAsyncLifetime`）。
- NUnit：老牌，`[Test]`/`[TestCase]`，参数化能力强。
- MSTest：VS 内置，`[TestMethod]`。
- 现代项目默认 xUnit。


**代码示例（xUnit）：**

```csharp
// xUnit：现代主流，[Fact]/[Theory]，扩展性好
public class CalculatorTests
{
    private readonly Calculator _calc = new();

    [Fact]                                   // 普通测试
    public void Add_TwoNumbers_ReturnsSum()
    {
        var result = _calc.Add(1, 2);
        Assert.Equal(3, result);
    }

    [Theory]                                 // 参数化测试
    [InlineData(1, 2, 3)]
    [InlineData(10, 20, 30)]
    public void Add_Parameters_ReturnsSum(int a, int b, int expected)
    {
        Assert.Equal(expected, _calc.Add(a, b));
    }
}

// 共享上下文：IClassFixture<T>（每个测试类一次）、IAsyncLifetime（异步初始化）
public class Calculator { public int Add(int a, int b) => a + b; }

// NUnit：[Test]/[TestCase]；MSTest：[TestMethod]——现代项目默认 xUnit
```

### 11.3 什么是 AAA 模式？

- **Arrange**（准备数据/依赖）→ **Act**（执行被测代码）→ **Assert**（断言结果）。
- 每个测试只测一件事、命名清晰（`Method_Scenario_Expected`）。


**代码示例：**

```csharp
[Fact]
public void Calculate_Discount_ReturnsVipPrice()
{
    // Arrange：准备数据与依赖
    var pricer = new OrderPricer(new VipPricing());
    decimal basePrice = 100m;

    // Act：执行被测代码
    decimal result = pricer.Price(basePrice);

    // Assert：断言结果
    Assert.Equal(80m, result);
}

// 规范：每个测试只测一件事；命名 Method_Scenario_Expected
class OrderPricer(VipPricing strategy) { public decimal Price(decimal p) => strategy.Calc(p); }
class VipPricing { public decimal Calc(decimal p) => p * 0.8m; }
```

### 11.4 什么是 Mock？Moq 怎么用？

- Mock：替身对象模拟依赖行为，隔离被测单元。
- Moq：`var mock = new Mock<IUserRepo>();` `mock.Setup(x => x.Get(1)).Returns(user);` `mock.Verify(x => x.Get(1), Times.Once);`
- 现代替代：NSubstitute、FakeItEasy；也鼓励用**手写 Fake**（简单、无魔法）。
- 陷阱：过度 Mock 导致测试与实现耦合（测试白盒化）；尽量 Mock 边界（I/O），少 Mock 内部逻辑。


**代码示例（Moq）：**

```csharp
public interface IUserRepository { User? GetById(long id); }

[Fact]
public void GetUser_NotFound_ReturnsNull()
{
    // Arrange：Mock 依赖，隔离被测单元
    var mock = new Mock<IUserRepository>();
    mock.Setup(r => r.GetById(It.IsAny<long>()))
        .Returns((User?)null);

    var service = new UserService(mock.Object);

    // Act
    var user = service.Get(999);

    // Assert + Verify（验证依赖被正确调用）
    Assert.Null(user);
    mock.Verify(r => r.GetById(999), Times.Once);   // 恰好调用一次
}

public class UserService(IUserRepository repo)
{
    public User? Get(long id) => repo.GetById(id);
}
class User { public long Id { get; set; } }

// 现代替代：NSubstitute、FakeItEasy；简单场景可用手写 Fake
// 陷阱：过度 Mock 导致测试白盒化；尽量 Mock 边界（I/O）
```

### 11.5 什么是 `WebApplicationFactory<T>`？（高频）

- 集成测试入口：进程内启动完整 ASP.NET Core 应用（TestServer），真实管道 + 真实 DI，无需部署/端口。
- 能力：`ConfigureWebHost` 覆盖服务（替换 DB 为测试库/容器库）、`CreateClient()` 发真实 HTTP 请求。
- 用法：`IClassFixture<WebApplicationFactory<Program>>` 共享实例；配合 Testcontainers 起真实 Postgres/SQL Server。


**代码示例：**

```csharp
// 集成测试：进程内启动完整应用（TestServer），真实管道 + 真实 DI
public class OrderApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OrderApiTests(WebApplicationFactory<Program> factory)
    {
        // 覆盖服务：替换 DB 为测试库/容器库
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var db = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (db is not null) services.Remove(db);

                services.AddDbContext<AppDbContext>(o =>
                    o.UseSqlServer(TestDatabase.ConnectionString));  // 或 Testcontainers
            });
        }).CreateClient();                    // 发真实 HTTP 请求
    }

    [Fact]
    public async Task GetOrder_ReturnsOk()
    {
        var resp = await _client.GetAsync("/api/orders/1");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}

// 配合 Testcontainers 起真实 Postgres/SQL Server（见 11.6）
// 注意：需要 Program 为 public 或用 InternalsVisibleTo
```

### 11.6 什么是 Testcontainers？

- 测试中按需启动 Docker 容器（数据库、Redis、消息队列），用完销毁；集成测试环境与生产一致。
- 比内存数据库（EF InMemory/`SQLite InMemory`）更真实（约束、事务、并发行为一致）。

### 11.7 如何测试异步代码？

- `await` 被测方法；断言 `Task` 结果；异常用 `await Assert.ThrowsAsync<T>`。
- 避免在测试中 `.Wait()`/`.Result`；注意测试超时配置。


**代码示例：**

```csharp
public class AsyncService
{
    public async Task<int> AddAsync(int a, int b)
    {
        await Task.Delay(1);
        return a + b;
    }

    public async Task<int> DivideAsync(int a, int b)
    {
        await Task.Delay(1);
        if (b == 0) throw new DivideByZeroException();
        return a / b;
    }
}

[Fact]
public async Task AddAsync_ReturnsSum()
{
    var svc = new AsyncService();
    int result = await svc.AddAsync(1, 2);       // await 被测方法
    Assert.Equal(3, result);
}

[Fact]
public async Task DivideAsync_Zero_Throws()
{
    var svc = new AsyncService();
    await Assert.ThrowsAsync<DivideByZeroException>(   // 异步断言异常
        () => svc.DivideAsync(1, 0));
}

// 避免在测试中 .Wait()/.Result（会阻塞并可能死锁）
```

### 11.8 什么是代码覆盖率？如何提升？

- 覆盖率：被测试执行到的代码比例（行/分支）；工具：Coverlet + `dotnet test --collect:"XPlat Code Coverage"`。
- 覆盖率不是目标：关注关键分支/边界/异常路径；不要为凑指标写无效测试。

### 11.9 什么是 TDD？

- 测试驱动开发：先写失败测试 → 最小实现 → 重构（红-绿-重构循环）。
- 好处：接口先行、持续回归、设计驱动；业务不明确的场景 TDD 较难。

### 11.10 什么是契约测试（Contract Testing）？

- 验证服务间接口契约（Provider 与 Consumer 一致），防止上游改接口破坏下游；工具：Pact（.NET 有 PactNet）。
- 比 E2E 便宜、比单测更真实，适合微服务。

### 11.11 什么是冒烟测试、回归测试、E2E 测试？

- 冒烟：核心路径快速验证（部署后跑通主要流程）。
- 回归：改动后验证旧功能未坏。
- E2E：端到端（浏览器/真实环境），工具 Playwright、Selenium。

### 11.12 如何测试 EF Core 数据层？

- 推荐：真实数据库（Testcontainers）+ 每次测试独立事务/回滚或清库；`Respawn` 重置数据库。
- 备选：SQLite InMemory（限制：行为差异）或 InMemory Provider（**仅用于逻辑测试，不验证 SQL**）。

### 11.13 什么是依赖注入的可测试性？

- 依赖通过构造函数注入接口 → 测试时可替换为 Mock/Fake；避免静态方法、ServiceLocator、new 内部依赖。
- 例：`TaxCalculator` 依赖 `ICurrencyConverter` 接口，测试注入 Fake。

### 11.14 什么是 `Assert` 与 `FluentAssertions`？

- xUnit `Assert.Equal` 等；FluentAssertions：`result.Should().Be(42)`，断言可读性更好、错误信息更友好。


**代码示例：**

```csharp
// xUnit 原生断言
Assert.Equal(3, result);
Assert.True(flag);
Assert.NotNull(obj);

// FluentAssertions：可读性更好、错误信息更友好
result.Should().Be(3);
result.Should().BeGreaterThan(0).And.BeLessThan(10);   // 链式断言
list.Should().HaveCount(3).And.Contain(2);
ex.Should().Throw<DivideByZeroException>();            // 异常断言
str.Should().StartWith("Hello").And.EndWith("!");

// 安装：dotnet add package FluentAssertions
// 适合验收/业务规则复杂的测试，断言即文档
```

---

<a id="c12"></a>

## 十二、综合场景与行为面试

### 12.1 请介绍一个你最有成就感的项目？（几乎必问）

**回答框架（STAR）**：
- **S（情境）**：项目背景、你的角色。
- **T（任务）**：你负责的核心目标。
- **A（行动）**：技术选型与关键动作（架构、优化、攻坚点）——突出技术深度。
- **R（结果）**：量化结果（QPS 提升 X 倍、耗时从 X→Y、支撑 XX 用户）。
- 加分：说明你遇到的**难点**（高并发、一致性、性能、跨团队协作）与**取舍**（为什么这么做，不这么做会怎样）。

### 12.2 设计一个高并发秒杀系统（经典场景题）

**要点（分层防护）：**
1. **前端/网关**：限流、答题/验证码、按钮置灰、CDN 静态化；
2. **库存预减**：Redis `DECR` 原子扣减（超卖防护），扣减成功才放行；
3. **异步下单**：请求入消息队列，消费者串行/批处理落库；
4. **数据库**：乐观锁/行锁防超卖；订单号唯一约束防重复；
5. **幂等**：用户+场次唯一键，防止重复下单；
6. **风控/黑名单**、监控告警、降级开关。
- 追问点：超卖怎么防？（Redis 原子 + 数据库兜底）；消息积压怎么办？（水平扩容消费者、削峰）；缓存雪崩/穿透？


**代码示例（核心防护伪代码）：**

```csharp
// 1. 前端/网关限流 + 按钮置灰 + 验证码（防脚本刷单）
// 2. 库存预减：Redis 原子扣减（超卖防护的第一道闸）
public async Task<bool> TryDeductStockAsync(string sku, int qty)
{
    // SET 库存时用 Lua 保证"检查-扣减"原子性
    const string lua = """
        local stock = tonumber(redis.call('get', KEYS[1]) or '0')
        if stock >= tonumber(ARGV[1]) then
            redis.call('decrby', KEYS[1], ARGV[1])
            return 1
        end
        return 0
        """;
    var ok = await db.ScriptEvaluateAsync(lua,
        new RedisKey[] { $"stock:{sku}" }, new RedisValue[] { qty });
    return (long)ok! == 1;
}

// 3. 扣减成功才放行：请求入消息队列，消费者串行/批处理落库
await channel.Writer.WriteAsync(new SeckillRequest(userId, sku, qty));

// 4. 消费者落库：数据库乐观锁/行锁兜底防超卖 + 订单唯一约束防重复
//    UPDATE Product SET Stock = Stock - @qty
//    WHERE Id = @id AND Stock >= @qty;   -- 影响行数 0 => 卖完

// 5. 幂等：用户+场次唯一键（唯一索引），重复请求返回首次结果
// 6. 风控/黑名单、监控告警、降级开关

// 追问：消息积压？-> 水平扩容消费者、削峰；缓存穿透/雪崩？-> 见 5.22
```

### 12.3 如何设计一个分布式锁？（见 7.13，场景化回答）

- 选型：Redis `SET NX PX` + Lua 释放 + 看门狗续期；权衡主从切换下的可靠性（RedLock）或改用 ZooKeeper/数据库。
- 结合项目讲：抢购防重入、定时任务防重复执行、库存扣减。

### 12.4 线上接口变慢/超时，如何排查？（高频场景）

**排查路径：**
1. 确认现象范围：单接口/全部？单实例/全集群？时间段？
2. 看监控：QPS、P99 延迟、CPU/内存/GC、连接池、线程池饥饿指标；
3. 应用层：慢日志、链路追踪（traceId 定位慢 Span）、线程 dump 找阻塞；
4. 数据库：慢查询日志、执行计划、锁等待、索引失效；
5. 依赖：外部 API 超时、Redis 慢命令、消息积压；
6. 定位后验证：优化 → 压测 → 观察指标回落。
- 加分：按"监控 → 分层定位 → 假设验证"的工程化方法回答。

### 12.5 线上出现内存暴涨/OOM，如何处理？

1. 确认是托管堆还是本机内存（`dotnet-counters`、`/proc`、任务管理器）；
2. 抓 `dotnet-gcdump`/`dotnet-dump` 分析**存活对象增长**（对比前后快照）；
3. 排查根：事件订阅、静态缓存、未释放资源、LOH 碎片；
4. 应急：先扩容/重启（治标）+ 修代码（治本）；压测回归。
- 注意：不要只看 working set（.NET 不主动归还内存）；重点看**存活堆**趋势。

### 12.6 如何设计一个短链接系统（URL Shortener）？（系统设计）

- 思路：全局唯一 ID（雪花/发号器）→ Base62 编码短码；存储：DB（id + 长链）+ Redis 缓存 + 布隆过滤防穿透。
- 读写：写少读多；读走缓存，热点打散；跳转 302/301（301 被浏览器缓存难统计，302 常用）。
- 扩展：统计点击、过期策略、防滥用。

### 12.7 如何设计一个限流器（Rate Limiter）？

- 算法：固定窗口、滑动窗口、令牌桶、漏桶（Leaky Bucket）——各自特点与取舍。
- 单机：内存计数（简单）；分布式：Redis `INCR` + 过期时间 / Lua 脚本。
- 落地：网关层 + 应用层双层；按用户/IP/接口维度；返回 429 + Retry-After。


**代码示例（令牌桶 + 分布式）：**

```csharp
// 令牌桶算法（单机内存版）
public class TokenBucket
{
    private readonly double _rate;        // 每秒补充令牌数
    private readonly int _capacity;       // 桶容量（突发上限）
    private double _tokens;
    private long _lastRefill = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public TokenBucket(double rate, int capacity)
    {
        _rate = rate;
        _capacity = capacity;
        _tokens = capacity;
    }

    public bool TryAcquire()
    {
        lock (this)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _tokens = Math.Min(_capacity,
                _tokens + (now - _lastRefill) * _rate);   // 按时间补充
            _lastRefill = now;

            if (_tokens >= 1) { _tokens -= 1; return true; }
            return false;                                 // 拒绝，返回 429
        }
    }
}

// 分布式版：Redis Lua（原子 INCR + 过期窗口）或 .NET 内置 AddRateLimiter（见 5.32）
// 落地：网关层 + 应用层双层；按用户/IP/接口维度；返回 429 + Retry-After

// 固定窗口 / 滑动窗口 / 漏桶：各自特点——窗口类简单有突刺，令牌桶平滑
```

### 12.8 你如何看待技术债务？如何推进代码质量？

- 承认债务不可避免：区分"刻意债务"（短期交付，有明确偿还计划）与"无意债务"；
- 实践：代码评审、单元测试、CI/CD、重构小步走、静态分析（SonarQube）、规范文档；
- 回答体现工程化思维：质量是持续过程而非一次性。

### 12.9 你的职业规划是什么？为什么想加入我们公司？（行为）

- 结合技术方向（如深耕 .NET/云原生/架构）+ 与公司业务/技术栈匹配点；
- 具体、真实、可执行（短期 1-2 年、中期 3-5 年），避免空话。

### 12.10 你最近在学什么新技术？怎么学的？（高频）

- 说出具体主题（如 .NET 8/9 新特性、OpenTelemetry、AOT、MassTransit）+ 学习方式（官方文档、源码阅读、动手 Demo、社区文章、博客输出）。
- 加分：展示持续学习习惯与深度（看过源码、写过笔记、有开源贡献）。

### 12.11 如果你和同事对技术方案有分歧怎么办？（行为）

- 基于**数据/事实**讨论（压测、成本、风险），而不是立场；小范围验证（POC）；尊重决策并执行，事后复盘；不搞人身攻击。

### 12.12 谈一谈你对"重构"的理解？

- 重构 = 不改变外部行为的前提下改善内部结构（安全前提是测试保护）；
- 小步重构（每个步骤可运行）、持续重构优于大爆炸重写；重写要考虑机会成本与风险（绞杀者模式）。

---

<a id="c13"></a>

## 十三、附录：高频速记表与推荐资料

### 13.1 必背速记表

| 主题 | 一句话答案 |
|---|---|
| 值类型 vs 引用类型 | 值类型存值在栈（或内联），引用类型存地址指向堆对象 |
| 装箱拆箱 | 值→object 是装箱（堆分配），反之为拆箱；用泛型避免 |
| string vs StringBuilder | string 不可变，频繁拼接用 StringBuilder |
| GC 工作原理 | 分代 + 标记清除 + 压缩 + 终结器；托管内存自动管理 |
| 委托 vs 事件 | 事件是委托的封装，外部只能订阅不能触发 |
| IEnumerable vs IQueryable | 前者内存中遍历，后者可翻译成 SQL 在数据库执行 |
| async/await 原理 | 编译器生成状态机，await 挂起释放线程，完成后续延 |
| ConfigureAwait(false) | 不捕获同步上下文，类库代码防死锁、减开销 |
| sync-over-async 死锁 | 同步阻塞 + 上下文续延互等；async 一路向上 |
| async void 危险 | 无法 await、异常无法捕获导致崩溃；仅事件处理器可用 |
| DI 三种生命周期 | Transient 每次新建 / Scoped 每请求 / Singleton 全局唯一 |
| 生命周期倒挂 | Singleton 依赖 Scoped 是 Captive Dependency，危险 |
| 中间件顺序 | 认证在授权前，异常处理靠前；管道洋葱模型 |
| JWT 三段 | Header.Payload.Signature，签名防篡改，无状态 |
| 缓存穿透/击穿/雪崩 | 空值+布隆 / 互斥锁 / TTL 随机化+多级缓存 |
| EF N+1 | Include/投影/显式加载；懒加载慎用 |
| AsNoTracking | 只读查询不追踪，省内存提性能 |
| 乐观并发 | RowVersion/并发令牌，UPDATE 行数为 0 即冲突 |
| 事务隔离级别 | Read Uncommitted → Serializable 一致性升并发降 |
| SQL 索引 | 聚集索引定物理顺序（唯一），非聚集需回表，避免函数包列 |
| SOLID | 单职、开闭、里氏、接口隔离、依赖倒置 |
| 单例实现 | Lazy<T> 最简洁；现代用 AddSingleton |
| 消息可靠性 | 生产者确认 + Broker 持久化 + 手动确认 + 幂等消费 |
| Outbox | 本地事务写 outbox 表 + Relay 发消息，保证最终一致 |
| Saga | 本地事务链 + 补偿，替代分布式事务 |
| 熔断/限流/降级 | Polly + AddRateLimiter；保护下游与自身 |
| .NET 内存泄漏 | 事件未退订、静态缓存无限、未释放资源 |
| 排查工具 | dotnet-counters / gcdump / dump / trace |
| HttpClient | 用 IHttpClientFactory，避免 Socket 耗尽 |
| 单元 vs 集成 | 单测隔离依赖全内存；集成测真实协作 |
| WebApplicationFactory | 进程内启动真实管道做集成测试 |
| .NET 8+ 新特性 | 主构造函数、集合表达式、AOT、键控服务、Resilience |

### 13.2 面试回答万能公式（结构）

每道技术题按以下结构回答：
1. **是什么**：先给定义（一句话）；
2. **为什么**：解决什么问题 / 背景；
3. **怎么做**：具体用法、代码、配置；
4. **项目里怎么用**：结合实战案例；
5. **补充**：性能、安全、边界、反模式。

### 13.3 常见追问方向（提前准备）

- 你的回答中出现的每个概念都可能被追问"为什么/底层/权衡/反例"，**只写自己真正理解的内容**；
- 每个技术点准备 30 秒版本（电梯陈述）与 3 分钟版本（展开细节）；
- 手写代码准备：单例、快速排序/二分、字符串反转、生产者消费者、LINQ 模拟、异步死锁示例。

### 13.4 推荐学习资料

**官方文档**
- Microsoft Learn .NET：https://learn.microsoft.com/dotnet
- C# 语言文档：https://learn.microsoft.com/dotnet/csharp
- ASP.NET Core 文档：https://learn.microsoft.com/aspnet/core
- EF Core 文档：https://learn.microsoft.com/ef/core
- .NET API 参考：https://learn.microsoft.com/dotnet/api

**社区与博客**
- .NET 官方博客：https://devblogs.microsoft.com/dotnet/
- Andrew Lock（.NET 博客）：https://andrewlock.net/
- Nick Chapsas（YouTube/Blog）：https://www.youtube.com/@nickchapsas
- Stephen Cleary（异步编程）：https://blog.stephencleary.com/
- 中文社区：博客园 .NET 板块、掘金、知乎 .NET 话题、CSDN .NET 专栏

**书籍**
- 《CLR via C#》（Jeffrey Richter）——运行时与内存原理经典
- 《C# in Depth》（Jon Skeet）——语言特性深入
- 《Concurrency in C# Cookbook》（Stephen Cleary）——异步与并发
- 《.NET Microservices: Architecture for Containerized .NET Applications》（微软电子书）
- 《Design Patterns》（GoF）——设计模式经典

### 13.5 本题库参考来源（公开网页检索所得）

- CSDN《ASP.NET Core 面试题汇总》《ASP.NET Core 面试八股高频题 + 标准答案精简版》《C# 面试题及答案整理》
- codewithmukesh.com：.NET / ASP.NET Core / EF Core / Web API / Senior .NET 面试题系列（2026）
- codingdroplets.com：C# 多线程并发、Async/Await、EF Core、ASP.NET Core DI、测试系列（2026）
- learnixo.io：.NET/C# 中高级面试题（Q61–Q250）、Senior 完整指南
- interviewkickstart.com：.NET / C# 面试题
- simplilearn.com：Top 55+ C# 面试题
- techbeamers.com：20 Common .NET Coding Interview Questions
- bool.dev / jopr.org：.NET 面试题（800+）、微服务与分布式、SQL for .NET
- techprep.app：70 EF Core 面试题；lastroundai.com：.NET 开发者面试题
- stacklesson.com / voidgeeks.com / upGrad：C#/SQL Server/EF 高级面试题
- zeecod.com：Advanced .NET Internals；toptal.com：78 Essential .NET Interview Questions
- Microsoft Learn（官方）：事务隔离级别、异步编程、内存管理、DI、测试 ASP.NET Core、C# 语言参考（record、主构造函数）

---

> 提示：面试题答案具有时效性（如 .NET 10、C# 14 已发布，文中以 .NET 8/9/10 与 C# 12/13 为主），建议面试前结合官方文档核对最新版本特性。祝你面试顺利！
