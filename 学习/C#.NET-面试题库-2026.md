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

### 1.3 `string` 与 `StringBuilder` 的区别？

- `string` **不可变**，任何修改都会创建新字符串对象；大量拼接会产生大量垃圾对象。
- `StringBuilder` 是可变字符缓冲区，适合**循环内大量拼接**场景。
- 少量拼接（如 3~5 次）编译器/运行时已优化（字符串字面量拼接编译期完成），用 `string` 即可。
- 建议：循环拼接用 `StringBuilder`；一次性拼接可用 `$` 插值（内部走 `DefaultInterpolatedStringHandler`，已优化）。

### 1.4 `ref`、`out`、`in` 参数的区别？

| 关键字 | 传参前是否必须初始化 | 方法内是否必须赋值 | 用途 |
|---|---|---|---|
| `ref` | 必须 | 不必 | 按引用传递，读改写 |
| `out` | 不必 | 必须 | 多个返回值 |
| `in` | 必须 | 不能（只读） | 按引用传递只读，避免大结构体拷贝 |

**加分点**：`ref`/`in` 对**大 struct** 传参可避免拷贝提升性能；`in` 常用于性能敏感路径；值类型按引用传递后对参数赋值会修改调用方变量。

### 1.5 可空类型（`Nullable<T>` / `int?`）是什么？

- 值类型不能为 `null`，可空类型允许值类型表示"有值/无值"（如数据库空列）。
- 本质是 `Nullable<T>` 结构体，包含 `HasValue` 与 `Value` 两个属性。
- 运算符：`??`（空合并）、`?.`（空条件访问）、`??=`（空合并赋值）。
- 与数据库、JSON 交互时很常用；`null` 与 `0` 语义要区分清楚。

### 1.6 抽象类（abstract class）与接口（interface）的区别？（必问）

**要点：**
- **抽象类**：可以有**字段、构造函数、方法实现**；一个类**只能继承一个**抽象类；表示"是什么"（is-a）。
- **接口**：C# 8+ 可以有默认实现，但**不能有字段**（可有静态成员/属性）；一个类**可实现多个**接口；表示"能做什么"（can-do）。
- **选择**：有共享状态/实现逻辑用抽象类；定义契约、多行为组合用接口。
- 抽象类需 `abstract` 关键字，可含抽象方法（子类必须实现）；接口方法默认 `public`。

### 1.7 方法重载（Overload）与重写（Override）的区别？

- **重载**：同一个类中，方法名相同、**参数列表不同**，编译期决定，与返回类型无关。
- **重写**：子类用 `override` 重新实现父类的 `virtual`/`abstract` 方法，签名一致，**运行时多态**。
- `new` 关键字可隐藏父类方法（非多态），要区分。

### 1.8 `virtual`、`override`、`abstract`、`sealed`、`static` 的含义？

- `virtual`：允许子类重写的方法。
- `override`：子类重写父类虚方法/抽象方法。
- `abstract`：抽象成员/类，不能实例化，派生类必须实现。
- `sealed`：密封类不可继承；密封方法不可再重写。
- `static`：静态成员属于类型本身，所有实例共享；静态类不能实例化、不能被继承。

### 1.9 泛型（Generic）是什么？有什么好处？什么是泛型约束？

**要点：**
- 泛型把**类型参数化**，在编译期确定类型，避免装箱、保证类型安全、代码复用。
- 泛型约束：`where T : class / struct / new() / 基类 / 接口 / notnull / unmanaged` 等。
- 例子：`List<T>`、`Dictionary<K,V>`、`IEnumerable<T>`。
- 泛型类、泛型方法、泛型接口；协变 `out T`（`IEnumerable<out T>`）与逆变 `in T`（`IComparer<in T>`）。

### 1.10 什么是委托（delegate）？多播委托是什么？

- 委托是**方法引用的类型**，相当于类型安全的函数指针；可把方法当作参数传递。
- 内置委托：`Action`（无返回值）、`Func<T>`（有返回值）、`Predicate<T>`（返回 bool）。
- **多播委托**：`+=` 链式绑定多个方法，调用时按顺序执行；`-=` 移除；返回值取最后一个方法的返回值。
- 匿名方法、Lambda 表达式是委托的简洁写法。

### 1.11 事件（event）与委托（delegate）的关系和区别？（高频）

**要点：**
- 事件**基于委托**，是委托的封装，用于发布/订阅（观察者模式）。
- **区别**：
  1. 事件只能在**声明它的类内部触发**（`Invoke`），外部只能 `+=`/`-=` 订阅；委托可任意赋值调用。
  2. 事件字段默认 `private` 的委托实例 + 外部订阅接口，安全性更高。
  3. 事件强调"状态变化通知"，委托强调"回调抽象"。
- 标准事件模式：`EventHandler<TEventArgs>` + `sender` 参数。

### 1.12 什么是 Lambda 表达式？闭包（Closure）是什么？

- Lambda：`(x) => x * 2`，匿名函数简洁语法，可转为委托或表达式树。
- **闭包**：Lambda 捕获外部变量，该变量被提升为编译器生成的类的字段，生命周期延长到委托被释放。
- **陷阱**：`for` 循环中捕获循环变量——旧版本 C# 捕获的是同一个变量，C# 5 起 `foreach` 每次迭代的变量是新的；`for` 循环的索引仍需要注意。

### 1.13 什么是 LINQ？它的工作原理是什么？

**要点：**
- LINQ（Language Integrated Query）：统一的声明式查询语法，可查询集合、数据库、XML 等数据源。
- 两类语法：查询语法（`from...where...select`）与方法语法（`.Where().Select()`）。
- 核心机制：
  1. **延迟执行（Deferred Execution）**：查询定义时不执行，遍历（`foreach`、`ToList()` 等）时才执行；
  2. **提供程序模型**：LINQ to Objects（`IEnumerable<T>`）、LINQ to Entities（`IQueryable<T>` + EF）、LINQ to XML；
  3. 链式组合：每次调用返回新的查询对象。

### 1.14 `IEnumerable<T>` 与 `IQueryable<T>` 的区别？（高频）

| 维度 | `IEnumerable<T>` | `IQueryable<T>` |
|---|---|---|
| 命名空间 | System.Collections | System.Linq |
| 执行方式 | 内存中遍历（LINQ to Objects） | 可被 Provider 翻译成 SQL（LINQ to Entities） |
| 延迟执行 | 是 | 是（更强） |
| 适用 | 内存集合 | 数据库查询（EF Core） |
| 性能 | 会把全部数据拉进内存 | 查询在数据库端执行，只返回结果 |

**加分点**：EF Core 中 `IQueryable` 上组合条件不会立即执行，`ToList()` 才真正发 SQL；先 `ToList()` 再 `Where` 会把过滤放到内存中（错误用法）。

### 1.15 什么是扩展方法（Extension Methods）？

- 静态类中的静态方法，第一个参数加 `this` 关键字，即可像实例方法一样调用。
- 不能访问被扩展类型的私有成员；本质上还是静态方法调用。
- 经典例子：LINQ 的 `Where`/`Select`、`string.IsNullOrEmpty` 扩展。
- 命名空间必须 `using` 后才能使用；若与实例方法同名，实例方法优先。

### 1.16 什么是匿名类型（Anonymous Type）？

- 用 `new { Name = "x", Age = 18 }` 创建，编译器自动生成类型，属性只读。
- 只能配合 `var` 使用；常用于 LINQ 投影（`Select(x => new { ... })`）。
- C# 7+ 支持元组（`(int Id, string Name)`）作为替代，更轻量。

### 1.17 `var`、`dynamic`、`object` 的区别？

- `var`：编译期**类型推断**，变量类型固定，只是不用显式写出。
- `object`：所有类型的基类，编译期类型检查存在（需要强转才能用具体成员），使用值类型会**装箱**。
- `dynamic`：编译期**不检查**，运行时动态解析（DLR），性能更低，适合 COM 互操作、反射、动态 JSON；错误推迟到运行时。
- 结论：能用 `var` 用 `var`；能用强类型不用 `object`；`dynamic` 慎用。

### 1.18 什么是异常处理？`try/catch/finally` 的执行顺序？`throw` vs `throw ex`？

- **执行顺序**：`try` → 发生异常进 `catch` → 无论是否异常都执行 `finally`（除非进程终止/`Environment.FailFast`）。
- **`throw;`**：保留原始异常堆栈；**`throw ex;`**：重置堆栈为当前抛出点（**不推荐**，会丢失原始调用链信息）。
- **自定义异常**：继承 `Exception`，命名以 `Exception` 结尾，建议实现 3 个构造函数。
- **最佳实践**：只捕获能处理的异常；用 `catch (SpecificException)`；别捕获 `Exception` 后静默吞掉；`finally` 中释放资源或用 `using`。

### 1.19 什么是 `using` 语句？`IDisposable` 是什么？

- `using` 语句在代码块结束时自动调用 `Dispose()`，用于确定性释放非托管资源（文件、连接、Socket）。
- `IDisposable`：实现 `Dispose()` 方法；`using var x = ...`（C# 8+）在作用域结束时释放。
- **陷阱**：`using` 不能保证进程崩溃时执行；`Dispose` 不应抛异常；注意与 `GC.SuppressFinalize(this)` 配合的 Dispose 模式。

### 1.20 什么是属性（Property）？自动属性与 `init` 只读属性？

- 属性是字段的封装，包含 `get`/`set` 访问器，可在其中加验证逻辑。
- 自动属性：`public int Age { get; set; }`，编译器生成后备字段。
- C# 9+ `init`：属性只能在**对象初始化器/构造函数**中赋值（不可变对象友好）。
- `required`（C# 11）：标记必填属性，编译期强制初始化。

### 1.21 什么是索引器（Indexer）？

- 让对象像数组一样用 `obj[index]` 访问：`public T this[int i] { get; set; }`。
- 可用于集合类、字典包装类。

### 1.22 什么是运算符重载？

- 用 `public static` 方法重载运算符（`+`、`-`、`==`、`<` 等）。
- 重载 `==` 时必须重载 `Equals` 和 `GetHashCode`。
- 常用于自定义数值类型、`Vector` 等。

### 1.23 `string` 相等比较：`==`、`Equals`、`ReferenceEquals`？

- `==`：`string` 类型重载后比较**内容**（值语义）。
- `Equals`：默认比较内容（`string` 重写后）。
- `ReferenceEquals`：比较引用地址。
- 区分大小写：默认区分；可用 `StringComparison.OrdinalIgnoreCase` / `StringComparer`。

### 1.24 什么是常量（const）与只读字段（readonly）的区别？

| 维度 | `const` | `readonly` |
|---|---|---|
| 赋值时机 | 声明时必须赋值（编译期常量） | 声明时或构造函数中 |
| 类型限制 | 只能是基元类型/string/enum | 任意类型 |
| 存储 | 编译期替换，属于类型 | 运行时赋值，实例字段 |
| static | 隐式 static | 实例成员（可 static readonly） |

### 1.25 什么是静态成员？静态类有什么特点？

- 静态成员属于类本身，所有实例共享，类加载时初始化。
- 静态类：不能实例化、不能继承、只能包含静态成员；如 `Math`、`Console`。
- 静态构造函数：类型首次被使用前执行一次，用于静态初始化。
- 静态字段生命周期 = 应用生命周期，**滥用会导致内存长期占用**。

### 1.26 什么是枚举（enum）？`[Flags]` 特性是什么？

- 枚举：一组命名常量，底层默认 `int`。
- `[Flags]`：位标志枚举，值应为 2 的幂，可用 `|` 组合、`&` 判断、`HasFlag()` 检查。
- 枚举转字符串/数字、解析：`Enum.Parse`、`ToString()`、`nameof`。

### 1.27 什么是结构体（struct）？什么时候用 struct 而不是 class？

- `struct` 是值类型，无继承（可实现接口）、无默认构造函数（C# 10+ 允许显式无参构造）、不应长期存在大对象。
- **用 struct 的场景**：小（≤16~24 字节）、不可变、短生命周期、不频繁装箱；如坐标、颜色、`Guid`。
- **用 class 的场景**：较大、需要多态/继承、作为引用共享。
- **陷阱**：struct 不能为 null（除非可空）；按值传递副本；修改副本不影响原对象。

### 1.28 什么是元组（Tuple）与值元组（ValueTuple）？

- `Tuple`（引用类型）：`Tuple.Create(1, "a")`，成员为 `Item1`...，可空，性能差。
- `ValueTuple`（值类型）：`(int Id, string Name) p = (1, "a")`，支持命名成员、解构 `var (id, name) = p`，性能更好。
- 现代代码用 `ValueTuple`；常用于多返回值。

### 1.29 什么是 `switch` 表达式与模式匹配？

- C# 8+ switch 表达式：`var result = x switch { 1 => "a", _ => "b" };`
- 模式匹配：类型模式 `x is int i`、属性模式、位置模式、关系模式 `> 10`、逻辑模式 `and/or/not`。
- 常量模式、空检查 `x is null`。

### 1.30 什么是类型转换？`as`、`is`、强制转换的区别？

- `is`：类型检查，返回 bool；C# 7+ 可 `is Type v` 同时声明变量（模式匹配）。
- `as`：安全转换，失败返回 null（**仅限引用类型**）。
- 强制转换 `(Type)x`：失败抛 `InvalidCastException`。
- `Convert`：类型转换 + 值转换（如字符串转数字）。

### 1.31 数组与集合的区别？常用集合类的特点？

- 数组：固定长度、连续内存、`int[]`；性能好但不可变长。
- `List<T>`：动态扩容数组。
- `Dictionary<K,V>`：哈希表，O(1) 查找。
- `HashSet<T>`：无重复集合，O(1) 去重/判断。
- `LinkedList<T>`：链表，插入删除快。
- `Queue<T>` / `Stack<T>`：FIFO / LIFO。
- `SortedDictionary` / `SortedList`：有序。
- **线程安全**：`ConcurrentDictionary`、`ConcurrentQueue`、`BlockingCollection`。

### 1.32 什么是 `Object` 的四个方法？

- `ToString()`、`Equals(object)`、`GetHashCode()`、`GetType()`。
- `Equals` 与 `GetHashCode` 约定：相等对象必须哈希一致；重写 `Equals` 必须重写 `GetHashCode`（否则字典/哈希集合行为错误）。

### 1.33 什么是不可变对象（Immutable）？有什么好处？

- 创建后状态不可变（只读字段、`init`、`record`）。
- 好处：线程安全、可安全共享、可缓存、适合函数式编程与并发。
- 缺点：频繁修改会产生新对象，注意性能。

### 1.34 什么是 `record`？与 `class` 的区别？（高频，C# 9+）

**要点：**
- `record` 是引用类型，但提供**值相等性**（基于属性逐个比较）和 `with` 表达式（非破坏性修改）。
- 编译器自动生成：`Equals`、`GetHashCode`、`ToString()`、`==`/`!=`、`Deconstruct`、克隆保护。
- `record struct`（C# 10+）：值类型记录。
- **使用场景**：DTO、不可变数据模型、值对象。
- **注意**：性能敏感场景 record 的相等性比较有开销；继承场景中相等性语义要小心。

### 1.35 什么是主构造函数（Primary Constructor）？（C# 12 / C# 13）

- C# 12：`public class OrderService(IOrderRepository repo) { ... }`，参数在类体内可用，简化 DI 构造。
- C# 13 增强：支持 `readonly` 修饰参数、隐式字段绑定，class/struct/record 语义有差异（record 参数自动生成属性参与值相等性比较；class 不自动生成属性，需自行赋值）。
- **注意**：C# 13 主构造参数默认生成隐式字段（若未显式使用）；不要重复声明字段导致编译警告。

### 1.36 什么是 `Span<T>` / `Memory<T>`？（高级）

- `Span<T>`：栈上分配的**连续内存切片**，无堆分配、无装箱，性能极高；不能跨越 `await`（ref struct）。
- `Memory<T>`：托管堆上的可变长度内存，可异步使用。
- 用于高性能解析、字符串处理、避免复制；`ReadOnlySpan<char>` 常用于字符串切片。

### 1.37 什么是 `yield return`？

- 迭代器：用 `yield return` 逐个产出元素，**惰性求值**——每次 `MoveNext()` 才执行到下一个 `yield`。
- 优点：不一次性生成全部数据，节省内存（大集合/流式处理）。
- 注意：迭代器方法中不能有 `ref`/`out` 参数；异常在遍历时抛出。

### 1.38 什么是命名空间（namespace）与 `using`？

- 命名空间组织类型、避免命名冲突；`using` 导入命名空间。
- `using static`（C# 6+）：直接引入静态成员。
- 全局 using（C# 10+）：`global using` 全项目生效。
- 文件级命名空间（C# 10+）：简化缩进。

### 1.39 C# 编译过程：源代码如何变成可执行程序？

1. 源代码 → 编译器（Roslyn）→ **IL（中间语言）** + 元数据，打包为程序集（.dll/.exe）；
2. 运行时 CLR 加载程序集 → **JIT 编译**成机器码 → 执行；
3. 可选：AOT（Native AOT，.NET 7+）提前编译为原生代码，启动更快、体积小，但反射/动态代码受限。

### 1.40 `#if` 预处理器指令是做什么的？

- 条件编译：`#if DEBUG ... #endif`、`#define`、`[Conditional("DEBUG")]`。
- 用于调试代码、平台差异代码；注意过度使用会降低可读性。

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

### 2.6 什么是特性（Attribute）？如何自定义？

- 特性是给代码元素（类、方法、属性）附加**元数据**的类，继承 `System.Attribute`。
- 内置：`[Obsolete]`、`[Serializable]`、`[Authorize]`、`[Route]`、`[ApiController]` 等。
- 自定义：`[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]` + 构造函数/命名参数。
- 读取：通过反射 `GetCustomAttribute` / `GetCustomAttributes`。

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

### 2.11 什么是序列化？有哪些方式？

- 序列化：对象 → 字节流/字符串（存储或传输）；反序列化逆向。
- 方式：`System.Text.Json`（推荐）、Newtonsoft.Json、XML（`XmlSerializer`）、二进制（BinaryFormatter **已废弃，有安全风险**）、`DataContractSerializer`。
- 注意循环引用、版本兼容、`[JsonIgnore]`、`[JsonPropertyName]`。

### 2.12 什么是源生成器（Source Generator）？（高级）

- 编译期分析代码并**生成额外 C# 源码**，减少反射开销、提升启动性能。
- 典型应用：`System.Text.Json` 源生成、`LoggerMessage` 高性能日志、`Regex` 源生成（`[GeneratedRegex]`）、依赖注入优化。
- 与反射对比：无运行时开销、AOT 友好。

### 2.13 什么是 `unsafe` 代码和指针？

- `unsafe` 允许使用指针、直接内存操作（`fixed` 固定对象、`stackalloc`）。
- 场景：高性能互操作、图像处理、算法库；需 `<AllowUnsafeBlocks>true`。
- **风险**：破坏类型安全，需严格控制作用域。

### 2.14 什么是 `InternalsVisibleTo`？

- 允许指定程序集访问本程序集 `internal` 成员，常用于**单元测试**（测试程序集访问内部实现）。

### 2.15 什么是 `StringBuilder` 底层原理？字符池（Interning）是什么？

- `StringBuilder` 内部是 `char[]` 缓冲，容量不足时翻倍扩容。
- 字符串驻留（Interning）：编译期/运行时字符串字面量在进程内共享（`String.Intern`），`==` 对驻留字符串比较引用即内容。动态拼接的字符串不驻留。

### 2.16 什么是 `CultureInfo` 与文化敏感操作？

- 区域文化影响格式化：日期 `ToString()`、数字小数点、货币。
- 服务端统一 `InvariantCulture` 或固定 `CultureInfo`，避免环境差异导致数据错误（如 SQL 日期转换）。

### 2.17 什么是弱引用（WeakReference）？

- 不阻止 GC 回收目标对象；`WeakReference` + 目标仍可访问（`IsAlive`/`Target`）。
- 用于大缓存：内存不足时缓存对象可被回收，避免内存泄漏。

### 2.18 什么是 `Conditional` 特性？

- `[Conditional("DEBUG")]` 标记的方法在指定符号未定义时**调用点被移除**，方法体仍需编译。
- 比 `#if` 更整洁，常用于调试辅助方法。

### 2.19 什么是文化无关字符串比较？

- `string.Compare(a, b, StringComparison.Ordinal)`：按字节序比较，性能高、确定性好。
- 推荐：内部标识符/路径用 `Ordinal`；用户文本排序用 `CurrentCulture`。

### 2.20 什么是 `Null` 传播与空值安全？

- `?.`（null 条件）、`??`（合并）、`??=`（赋值）、`!`（null 容忍运算符，声明"不为空"）。
- 可空引用类型（NRT）：`string?` 表示可能为 null，编译器做流分析警告。

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

### 3.3 什么是线程池（ThreadPool）？

- 线程池管理一组工作线程，任务提交后复用线程，避免频繁创建销毁线程的开销。
- 默认线程数：CPU 数 × 一些系数（随负载自适应）。
- **线程池饥饿（ThreadPool Starvation）**：线程被阻塞（如 sync-over-async、长阻塞 I/O、锁），任务排队等待，吞吐量骤降——表现为请求排队、超时。
- 最小线程数可配置：`ThreadPool.SetMinThreads`。

### 3.4 `ConfigureAwait(false)` 是做什么的？什么时候用？（高频）

- 默认 `await` 会捕获当前 `SynchronizationContext`（UI 线程/请求上下文），完成后回到原上下文执行。
- `ConfigureAwait(false)`：**不捕获上下文**，继续在任意线程池线程执行，避免死锁、减少切换开销。
- **使用场景**：类库代码、后台服务（无 UI 上下文需求）；**不要**在 UI 代码用（需要回到 UI 线程更新界面）。
- **现代 ASP.NET Core 中无 SynchronizationContext**，死锁风险低，但仍建议库代码使用以减开销。

### 3.5 什么是 sync-over-async 死锁？如何避免？（必问）

**要点：**
- 场景：UI 应用/旧 ASP.NET 中，同步上下文上调用 `.Result`/`.Wait()` 阻塞调用线程，而 await 的续延需要回到该上下文执行 → 互相等待 → 死锁。
- **避免**：
  1. async 一路向上，入口也异步；
  2. 需要桥接时：`Task.Run(() => AsyncMethod()).GetAwaiter().GetResult()`（脱离原上下文）；
  3. 类库内用 `ConfigureAwait(false)`。
- ASP.NET Core 中请求无同步上下文，但**阻塞线程池**同样降低吞吐，仍应避免。

### 3.6 为什么 `async void` 很危险？

- `async void` 无法被 `await`，异常无法被调用方捕获，会直接抛到 `SynchronizationContext` 导致**进程崩溃**。
- 仅限 UI 事件处理器使用（WinForms/WPF 事件签名要求 void）。
- 其余场景一律 `async Task` / `async Task<T>`。

### 3.7 `ValueTask<T>` 与 `Task<T>` 的区别？

- `Task<T>`：引用类型，结果可复用、多次 await；有堆分配。
- `ValueTask<T>`：值类型，避免分配；**只能 await 一次**（不可多次 await/缓存），适合结果往往同步可得的场景（如缓存命中、短操作）。
- 用法：`ValueTask<T>` 用于低频异步、结果通常立即可得的方法签名。

### 3.8 多线程与异步编程的区别？（高频）

**要点：**
- **多线程**：真正并行执行多个代码路径（每个线程独立），适合 CPU 密集；资源消耗大，需处理竞争/死锁。
- **异步**：单个线程可"挂起-继续"多个操作，适合 I/O 密集；不创建新线程，靠线程复用提高吞吐。
- 一句话：异步解决**等待**问题（不阻塞），多线程解决**并行计算**问题。
- 现代 .NET：I/O 用 async/await，CPU 并行用 `Parallel`/`PLINQ`/`Task.Run`。

### 3.9 什么是线程安全？`lock` 关键字如何工作？

- 线程安全：多个线程并发访问共享状态时结果正确。
- `lock (obj)` 是 `Monitor.Enter/Exit` 的语法糖，互斥访问临界区；对象引用作锁（不要锁值类型/字符串字面量）。
- `Monitor` 支持 `TryEnter` 超时；`lock` 不能跨 `await`（C# 编译报错）——异步互斥要用 `SemaphoreSlim`。
- 替代：`Interlocked`（原子操作）、`ReaderWriterLockSlim`（读写锁）、`SemaphoreSlim`、`Mutex`、`Concurrent*` 集合。

### 3.10 什么是死锁（Deadlock）？如何避免？

- 死锁：两个以上线程各自持有锁等待对方释放 → 永久阻塞。
- **必要条件**：互斥、持有并等待、不可剥夺、循环等待。
- **避免**：锁顺序一致、减少锁持有时间、用超时（`TryEnter`）、避免嵌套锁、用无锁结构/异步。

### 3.11 什么是竞争条件（Race Condition）？

- 多线程并发访问共享数据，结果依赖执行时序而不可预测（如 `count++` 非原子）。
- 解决：`lock`、`Interlocked.Increment`、`ConcurrentDictionary`、不可变数据、同步集合。

### 3.12 `volatile` 关键字的作用？

- 告诉编译器/运行时该字段每次读写都直接访问内存，禁止缓存优化，保证**可见性**（多线程读取最新值）。
- **不能保证原子性**：`volatile int` 的 `++` 仍非原子。
- 仅适用于基元类型（引用类型、int、bool 等）；复杂场景用 `Interlocked`/`lock`。

### 3.13 `Interlocked` 类提供什么能力？

- 原子操作：`Increment`、`Decrement`、`Add`、`Exchange`、`CompareExchange`。
- 无锁原子更新，性能高于 lock；典型：计数器、状态标志、`CompareExchange` 实现自旋锁。

### 3.14 什么是 `SemaphoreSlim`、`Mutex`、`ReaderWriterLockSlim`？

- `SemaphoreSlim`：信号量，限制同时访问的并发数；支持异步 `WaitAsync()`（**async 场景推荐**）。
- `Mutex`：跨进程互斥锁（系统级命名）；进程内优先用 `lock`。
- `ReaderWriterLockSlim`：多读单写；读多写少场景性能好。

### 3.15 并发集合有哪些？适用场景？

- `ConcurrentDictionary<TKey,T>`：并发字典（读多写多）。
- `ConcurrentQueue<T>`：线程安全 FIFO（生产者消费者）。
- `ConcurrentBag<T>`：无序集合（每个线程本地缓存）。
- `BlockingCollection<T>`：阻塞队列，支持 `CompleteAdding`、边界。
- `ConcurrentStack<T>`：LIFO。
- **注意**：普通 `Dictionary` + `lock` 在低并发下可能更简单；`ConcurrentDictionary` 的 `Count` 是近似值。

### 3.16 什么是 `Parallel`、`PLINQ`？

- `Parallel.For` / `Parallel.ForEach`：CPU 密集任务的并行执行（自动分块到多线程）。
- PLINQ：`.AsParallel().Where(...)` 并行 LINQ。
- 注意：I/O 密集不适合（应用异步）；并行有调度开销，任务太小反而更慢；结果合并需线程安全（`ConcurrentBag`、锁、`Aggregate`）。

### 3.17 什么是 `CancellationToken`？

- 协作式取消：`CancellationTokenSource` 发出取消信号，任务内检查 `token.IsCancellationRequested` 或调用 `ThrowIfCancellationRequested()`。
- 贯穿异步 API（`HttpClient`、EF Core、`Task.Delay`）；支持超时取消（`CancelAfter`）、链式取消（`CreateLinkedTokenSource`）。
- 取消是协作式：需要代码主动响应，不能强杀线程。

### 3.18 什么是 `TaskCompletionSource<T>`？

- 手动控制 `Task<T>` 的完成/失败/取消：`SetResult`、`SetException`、`SetCanceled`。
- 用于把回调/事件包装成 Task（Task-based 异步模式），如 `Socket` 回调封装、等待某事件触发。

### 3.19 `Task.Delay` 与 `Thread.Sleep` 的区别？

- `Thread.Sleep`：**阻塞**当前线程指定毫秒（浪费线程池线程）。
- `Task.Delay`：异步等待，不阻塞线程（返回 Task，可 await）；支持 `CancellationToken`。
- 服务端代码一律用 `Task.Delay`。

### 3.20 什么是生产者-消费者模式？在 .NET 中如何实现？

- 生产者生产数据、消费者处理数据，中间用缓冲队列解耦。
- 实现：`BlockingCollection<T>`、`Channel<T>`（推荐，异步友好，.NET Core 3+）、`ConcurrentQueue` + 信号量。
- 场景：消息处理、任务调度、日志批处理。

### 3.21 什么是 `Channel<T>`？

- 高性能异步管道：`Channel.CreateUnbounded/Bounded<T>()`，`WriteAsync`/`ReadAllAsync`。
- 支持背压（Bounded）、取消、完成；是现代 .NET 异步生产者-消费者的首选。

### 3.22 什么是数据竞争与内存可见性？

- 数据竞争：多个线程无同步地读写共享数据。
- 内存可见性：无同步时，一个线程的写可能对其他线程不可见（CPU 缓存/指令重排）；`lock`、`volatile`、`Interlocked` 建立内存屏障保证可见性。

### 3.23 什么是 `ThreadStatic` / `AsyncLocal<T>`？

- `[ThreadStatic]`：每个线程独立的静态字段副本。
- `AsyncLocal<T>`：**异步流**中传播的值（不随线程，随执行流），适合传递调用上下文（如 TraceId、租户信息）。
- 现代代码优先 `AsyncLocal`。

### 3.24 什么是 `IAsyncEnumerable<T>`？（C# 8+）

- 异步迭代器：`async` + `yield return`，配合 `await foreach` 消费。
- 场景：分页查询、流式处理、消息流（逐个异步产出元素，避免一次加载全部）。

### 3.25 什么是 `Task.WhenAll` / `Task.WhenAny` / `Task.WaitAll`？

- `WhenAll`：等待所有 Task 完成（异步，推荐），返回聚合结果；有异常聚合抛。
- `WhenAny`：任一完成即返回；用于超时竞争、首个响应。
- `WaitAll`：**阻塞**等待全部完成（避免在 async 上下文使用）。
- 注意：`WhenAll` 中某个 Task 失败会抛 `AggregateException`；可用 `WhenAll(ops.Select(o => o.ContinueWith(...)))` 容错。

### 3.26 什么是异步流？`await foreach`？

- 消费 `IAsyncEnumerable<T>`；每迭代一次异步获取一个元素。
- 例：`await foreach (var item in GetDataAsync()) { ... }`。

### 3.27 什么是"线程局部存储"？

- 每个线程独立存储空间（`ThreadLocal<T>`、`[ThreadStatic]`），避免共享冲突；`ThreadLocal<T>` 支持初始化和值工厂，且可释放。

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

### 4.4 什么是顶层语句（Top-level Statements）？

- C# 9+：`Program.cs` 中直接写代码，不用显式 `Main`/`class`，编译器自动生成入口点。
- 适合小型程序、脚本、`Program.cs` 简洁化；大型项目仍可用传统结构。

### 4.5 什么是 `System.Text.Json` 与 Newtonsoft.Json 的区别？

- `System.Text.Json`：官方内置、高性能、低分配；默认严格（大小写敏感）、特性支持有限。
- Newtonsoft.Json：老牌、功能全（`JsonConvert`、动态 `JObject`、条件序列化、循环引用处理），性能较慢。
- 选择：新项目默认 `System.Text.Json`；复杂序列化需求用 Newtonsoft。
- 性能优化：源生成（`JsonSerializerContext`）避免反射。

### 4.6 什么是 `Minimal API`？

- .NET 6+：`app.MapGet("/", () => "Hello")` 极简定义端点，不建 Controller。
- 适合小服务、微服务、函数、原型；性能略优（无 MVC 层开销）。
- 复杂项目仍用 Controller + MVC。

### 4.7 什么是 `WebApplication` 与 `WebApplicationBuilder`？

- `WebApplicationBuilder`：配置宿主（服务注册、配置、日志），`builder.Build()` 生成 `WebApplication`。
- `WebApplication`：既是应用又是中间件管道，`app.MapGet`、`app.Use...`、`app.Run()`。
- 这是 .NET 6+ 默认的最小宿主模型。

### 4.8 什么是 `IHost` / `IHostedService` / `BackgroundService`？

- `IHost`：通用主机，承载后台服务与应用生命周期。
- `IHostedService`：接口含 `StartAsync`/`StopAsync`，宿主启动/停止时执行。
- `BackgroundService`：`ExecuteAsync` 抽象基类，长期运行的后台任务（队列消费、定时任务、消息监听）。
- 注册：`builder.Services.AddHostedService<T>()`。

### 4.9 什么是 `Options` 模式（`IOptions<T>`）？

- 强类型配置：`services.Configure<T>(Configuration.GetSection("X"))`。
- `IOptions<T>`：应用启动时读取一次（不可重载）；`IOptionsSnapshot<T>`：每次请求读取（支持配置重载）；`IOptionsMonitor<T>`：监听变化。
- 验证：`[Required]`/`[Range]` 数据注解 + `ValidateOnStart`。

### 4.10 什么是配置系统（Configuration）？

- 分层配置源：appsettings.json、环境变量、命令行、用户机密、Azure Key Vault 等，按**注册顺序后者覆盖前者**。
- `IConfiguration` 按 `Section:Key` 访问；`GetSection`、`GetValue<T>`、`Bind`。
- 环境区分：`ASPNETCORE_ENVIRONMENT`（Development/Production）。

### 4.11 什么是依赖注入（DI）？什么是 IoC？（必问）

**要点：**
- **IoC（控制反转）**：对象创建和依赖关系的控制权从使用方反转给容器。
- **DI（依赖注入）**：IoC 的实现方式——通过构造函数/属性/方法把依赖传入，而不是类内部 `new`。
- **好处**：降低耦合、便于单元测试（mock 替换）、便于更换实现、生命周期统一管理。
- **反模式**：`ServiceLocator`（到处从容器取服务）、构造函数依赖过多（>4 个考虑重构）、静态服务。

### 4.12 什么是 `NuGet`？

- .NET 包管理器：`Install-Package` / `dotnet add package`；包源（nuget.org、私有源）。
- 解决依赖传递、版本冲突（`NU1605` 等）；`PackageReference` 声明在 .csproj。

### 4.13 什么是 `csproj` 的 SDK 风格项目？

- 简洁的项目文件：`<Project Sdk="Microsoft.NET.Sdk">`，默认包含所有 .cs 文件。
- 常用属性：`TargetFramework`、`Nullable`、`ImplicitUsings`、`LangVersion`。

### 4.14 什么是 `global.json`？

- 指定项目使用的 SDK 版本：`dotnet new globaljson`；保证团队构建一致性。

### 4.15 什么是 `dotnet` CLI 常用命令？

- `dotnet new`（模板）、`dotnet build`、`dotnet run`、`dotnet test`、`dotnet publish`（发布）、`dotnet restore`、`dotnet ef`（EF 工具）、`dotnet watch`（热重载）。

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

### 5.3 什么是中间件（Middleware）？（必问）

**要点：**
- 中间件是处理 HTTP 请求/响应的组件，组成**管道**：请求按注册顺序流入，响应按反方向返回。
- 每个中间件可：处理请求、调用下一个（`next()`）、处理响应、短路（不调 next）。
- 典型中间件：异常处理、认证、授权、静态文件、路由、日志、CORS、HTTPS 重定向。
- 顺序至关重要：`UseAuthentication` 必须在 `UseAuthorization` 之前；异常处理中间件尽量靠前；`UseRouting` 先于端点执行。

### 5.4 `Use`、`Run`、`Map` 的区别？

- **`Use`**：注册可传递的中间件（调用 `next` 继续）；可短路。
- **`Run`**：终止管道，不调用 next（终点站）。
- **`Map`**：按路径分支（`/admin` 走另一管道）；`MapWhen` 按条件分支。
- 例：`app.Map("/health", h => h.Run(...))`。

### 5.5 什么是依赖注入（DI）？三种生命周期？（必问）

**要点：**
- DI：对象的创建与依赖交给容器管理，通过构造函数注入。
- **生命周期**：
  - `Transient`：**每次解析**都新建实例（轻量无状态服务）；
  - `Scoped`：**每个请求/Scope 内**一个实例（DbContext、UnitOfWork）；
  - `Singleton`：**整个应用**一个实例（无状态工具类、缓存、配置）。
- **生命周期倒挂（Captive Dependency）**：Singleton 依赖 Scoped/Transient 是危险的——Singleton 会长期持有请求级实例（如 DbContext），导致状态混乱、连接/变更跟踪器泄漏，甚至运行时报错（验证模式可发现）。反之（Scoped 注入 Singleton）安全。

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

### 5.8 什么是 Model Binding？有哪些来源？

- Model Binding：把 HTTP 请求数据自动绑定到控制器参数/模型属性。
- 来源：路由（`[FromRoute]`）、Query（`[FromQuery]`）、Body（`[FromBody]`）、Form（`[FromForm]`）、Header（`[FromHeader]`）、Services（`[FromServices]`）。

### 5.9 什么是 Model Validation？

- 数据注解：`[Required]`、`[StringLength]`、`[Range]`、`[EmailAddress]`、`[RegularExpression]`、自定义 `ValidationAttribute`。
- `[ApiController]` 下自动验证并返回 400；非 API 控制器用 `ModelState.IsValid` 手动判断。
- 也可用 FluentValidation 做复杂校验。

### 5.10 `IActionResult` 与 `ActionResult<T>` 的区别？

- `IActionResult`：返回多种结果类型（`Ok()`、`NotFound()`、`BadRequest()`、`File()` 等），灵活但无类型约束。
- `ActionResult<T>`：约束返回 `T` 或状态码结果，Swagger 文档更准确；Web API 推荐。
- 都是 `Task<...>` 异步包装的返回值。

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

### 5.25 ASP.NET Core 中如何做日志？

- 内置 `ILogger<T>`，级别：Trace < Debug < Information < Warning < Error < Critical。
- 结构化日志：模板化消息（`_logger.LogWarning("User {UserId} ...", id)`）输出 JSON 字段，便于检索。
- 可接 Serilog、Seq、ELK、OpenTelemetry 做集中式日志与链路追踪。
- 高性能日志：`LoggerMessage` 源生成（`[LoggerMessage]`）避免装箱与模板解析开销。

### 5.26 什么是 Health Check？如何实现？

- 健康检查：探活应用及依赖（DB、Redis、外部 API）状态，供负载均衡/容器编排（K8s liveness/readiness）使用。
- 实现：`AddHealthChecks()` + `MapHealthChecks("/health")` + `AddDbContextCheck`/`AddRedis` 等；可自定义 `IHealthCheck`。
- 区分 liveness（进程存活）与 readiness（能否接流量）。

### 5.27 什么是接口幂等性？如何实现？（高频）

- **幂等**：同一请求执行多次与执行一次效果相同（如重复支付不重复扣款）。
- 实现：
  1. 请求携带**唯一请求 ID**，服务端记录处理结果，重复请求直接返回首次结果；
  2. 数据库唯一约束兜底（如订单号）；
  3. 乐观锁/版本号；
  4. 消息队列场景：消费方按消息 ID 去重。
- 注意 HTTP 语义：GET/PUT/DELETE 天然幂等，POST 不幂等。

### 5.28 什么是统一返回格式？

- 约定包装：`{ code, message, data }` 或 `{ success, error, data }`，配合全局异常处理和模型验证统一格式。
- 实现：`ActionResult` 包装器 + 过滤器/中间件统一序列化；注意与 OpenAPI 文档的兼容性。
- 避免：把业务错误码与 HTTP 状态码完全混用（可自定义 `ProblemDetails`）。

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

### 5.31 什么是 gRPC？与 REST 的区别？

- gRPC：基于 HTTP/2 + Protobuf 的 RPC 框架，二进制序列化、双向流、强类型契约（`.proto`）。
- 与 REST 对比：性能更高、契约强、适合服务间通信；调试不如 REST 直观、浏览器支持弱（需 gRPC-Web）。
- .NET 场景：微服务内部调用首选 gRPC，外部 API 用 REST。

### 5.32 什么是限流（Rate Limiting）？如何实现？

- 限制客户端请求速率，防滥用与雪崩。
- .NET 7+ 内置：`AddRateLimiter` + 固定窗口/滑动窗口/令牌桶/并发限制策略；`EnableRateLimiting` 特性。
- 也可用网关层（Nginx、Ocelot、YARP）或 Redis 计数器实现分布式限流。

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

### 5.36 什么是后台任务（Hosted Service）？如何避免常见陷阱？

- `BackgroundService` 实现长期后台循环（队列消费、定时扫描）。
- **陷阱**：构造函数注入 Scoped 服务（应为 `IServiceScopeFactory` 每次循环创建 Scope，避免 captive dependency、ChangeTracker 无限增长）；正确处理 `CancellationToken`；异常捕获防止服务静默退出。

### 5.37 什么是端点路由（Endpoint Routing）？

- .NET 3+：路由与中间件分离，`UseRouting` 匹配端点（不执行），`UseEndpoints`/`MapControllers` 执行。
- 好处：中间件可同时服务 MVC、SignalR、gRPC、Health Check 等多种端点；`LinkGenerator` 生成 URL。

### 5.38 什么是 `Options` 验证与 `ValidateOnStart`？

- `[ValidateOnStart]`：应用启动时立即验证配置，避免配置错误延迟到请求期才发现。
- 数据注解 + `IOptionsValidateOptions` 自定义校验。

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

### 6.2 什么是 Code First 与 Database First？

- **Code First**：先写实体类，`Add-Migration` 生成迁移，`Update-Database` 创建/更新库；现代 .NET 默认。
- **Database First**：从已有库生成实体（Scaffold-DbContext）；适合存量库。
- 注意：迁移是版本化的 schema 变更管理，不可删除已发布的迁移。

### 6.3 什么是 Migration？常用命令？（必问）

- Migration：把模型变更翻译为数据库结构变更的脚本与快照，保证代码与库同步。
- 命令：`dotnet ef migrations add Xxx` → 生成 `Up()`/`Down()` + 快照；`dotnet ef database update` 应用；`dotnet ef migrations remove` 撤销未应用的。
- 生产发布：在 CI/CD 中以独立步骤执行 `database update`，避免多实例同时启动时竞争（不要依赖启动时 `Migrate()`）。

### 6.4 什么是 Change Tracking？实体状态有哪些？

- ChangeTracker 跟踪实体状态：`Added`、`Modified`、`Unchanged`、`Deleted`、`Detached`。
- `SaveChanges` 根据状态生成 INSERT/UPDATE/DELETE；支持 `Reload`、`Detach`。
- 追踪查询（默认）每行实体缓存快照，用于生成 UPDATE 的 SET 子句。

### 6.5 什么是 `AsNoTracking()`？什么时候用？（高频）

- 查询结果**不追踪**，减少内存与快照开销，提升查询性能。
- 适用：**只读查询**（报表、列表展示）；不追踪的实体不能直接 `SaveChanges` 更新。
- 对比：需要更新的实体要追踪（或用 `Update` 附加）；`AsNoTrackingWithIdentityResolution` 处理导航属性重复。

### 6.6 什么是延迟执行（Deferred Execution）？查询什么时候真正执行？（高频）

**要点：**
- 构建 `IQueryable` 不执行 SQL；**遍历/物化**时才执行。
- 触发执行的操作：`ToList()`/`ToArray()`/`ToDictionary()`、`First()`/`Single()`/`Count()`/`Any()`/`Max()` 等聚合、`foreach`、`async` 版本（`ToListAsync`）。
- 意义：可分段组合查询条件（`if` 加 Where）；**陷阱**：延迟执行在循环内重复枚举会导致重复查询；查询在迭代时才执行，注意 using 作用域。

### 6.7 什么是 N+1 查询？如何避免？（必问）

**要点：**
- N+1：先查 1 条主记录，再对每条记录各发 1 条查询加载导航属性（1000 条订单 = 1001 条 SQL）。
- **解决**：
  1. **贪婪加载（Eager Loading）**：`Include(o => o.Customer)`、`ThenInclude(o => o.Customer.Address)`；
  2. **显式加载（Explicit Loading）**：`Entry(x).Reference(...).LoadAsync()`；
  3. **投影（Projection）**：`Select` 只取需要的字段（推荐，避免整表字段）。
- **懒加载（Lazy Loading）**：默认关闭，需装包 + `virtual` 导航属性；生产环境**谨慎使用**（隐式触发 N+1）。

### 6.8 `Include` 与 `ThenInclude` 的作用？

- `Include`：加载一级导航属性（JOIN）。
- `ThenInclude`：加载下一级（`Include(o => o.Items).ThenInclude(i => i.Product)`）。
- 过滤 Include（EF Core 5+）：`Include(o => o.Items.Where(i => i.Qty > 1))`。

### 6.9 什么是 `AsSplitQuery()`？什么时候用？

- 默认单条 JOIN 大查询；`AsSplitQuery` 拆成多条查询（EF Core 5+），减少笛卡尔爆炸与结果集膨胀。
- 适用：多级 Include、一对多关系多时；代价是多次往返，需权衡。
- EF Core 8+ 有 `SplitQueryBehavior` 全局配置。

### 6.10 什么是投影查询（Projection）？为什么要用？

- `Select(x => new Dto { ... })` 只取需要的列，SQL 更小、避免整实体加载、减少传输量。
- 投影后是 DTO（匿名类型/记录），不追踪更高效。
- 陷阱：投影中调用客户端方法会导致"客户端评估"错误或性能差（EF Core 3+ 默认禁止隐式客户端评估）。

### 6.11 什么是并发控制？乐观并发如何实现？（高频）

**要点：**
- 并发控制解决多人同时修改同一行数据。
- **悲观并发**：`SELECT ... FOR UPDATE`/行锁，先锁后改，冲突少但并发低。
- **乐观并发**：修改时校验版本，冲突则抛 `DbUpdateConcurrencyException`。
  - 实现：`[Timestamp]`/`IsRowVersion()` 的 `byte[] RowVersion` 或 `IsConcurrencyToken()` 标记列；EF 生成 `WHERE ... AND RowVersion = @v`，受影响行数为 0 时冲突。
- 处理：捕获异常 → 读取最新值 → 提示用户重试或合并。

### 6.12 什么是软删除与全局查询过滤器？（高频）

- **软删除**：不物理删除，加 `IsDeleted` 标志，`Update` 置位。
- **全局查询过滤器**：`modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted)`——所有查询自动过滤，且不能忘记（防止查回已删数据）；也可用于**多租户**隔离。
- 注意：过滤器对 `Include` 导航也生效；需要绕过时用 `IgnoreQueryFilters()`。

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

### 6.15 什么是 `SaveChanges` 事务？如何显式使用事务？

- `SaveChanges` 默认在事务中执行多个变更（同上下文）。
- 显式：`context.Database.BeginTransaction()` + `Commit`/`Rollback`；`CreateExecutionStrategy` 处理**连接级重试**（`EnableRetryOnFailure`）。
- 跨库/跨服务事务：EF 的本地事务不能跨库（需分布式方案：本地消息表、Outbox、Saga）。

### 6.16 什么是 `ExecuteUpdate` / `ExecuteDelete`？（EF Core 7+）

- 直接生成 UPDATE/DELETE SQL，**不加载实体**，避免先查后改；性能大幅提升。
- 条件 + 表达式：`context.Users.Where(u => u.Inactive).ExecuteDelete()`。
- 对比：`SaveChanges` 适合少量实体与复杂状态流转。

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

### 6.22 什么是 `Owned` 类型（值对象映射）？

- `Owned Entity`：值对象（如 `Address`、`Money`）嵌入同一张表（或按类型拆表）。
- 配置：`OwnsOne`/`OwnsMany`；适合 DDD 值对象建模。

### 6.23 什么是 `Migrations` 的回滚？

- `dotnet ef database update PreviousMigration` 回滚到指定版本（执行 `Down()`）；注意破坏性变更（删列）的逆操作。

### 6.24 什么是数据注解 vs Fluent API？

- 数据注解：实体上 `[Key]`、`[Required]`、`[MaxLength]`、`[Index]`，简单直接。
- Fluent API：`OnModelCreating` 中集中配置，灵活（关系、索引、并发令牌、过滤器）；复杂模型推荐 Fluent API。

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

### 7.2 什么是 SARGable？为什么函数包裹列会走不到索引？

- SARGable：查询条件**可利用索引**（Index Seek）。
- **非 SARGable**：`WHERE YEAR(OrderDate) = 2024`、`WHERE Name LIKE '%xx%'`、`WHERE col + 1 = 10`——列被函数/运算包裹，无法用索引，退化为全表扫描。
- 改写：`WHERE OrderDate >= '2024-01-01' AND OrderDate < '2025-01-01'`；`LIKE 'xx%'` 可走索引。

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

### 7.6 脏读、不可重复读、幻读是什么？

- **脏读**：读到其他事务**未提交**的数据（可能回滚）。
- **不可重复读**：同一事务内两次读同一行，值被其他事务**修改**（UPDATE）。
- **幻读**：同一事务内两次范围查询，结果行数变化（其他事务 **INSERT** 了满足条件的行）。

### 7.7 乐观锁与悲观锁？

- 悲观锁：先锁后操作（`UPDLOCK`/`SELECT FOR UPDATE`），冲突少、并发低、易死锁。
- 乐观锁：不锁，更新时校验版本/条件（`WHERE version = ?`），冲突抛错重试；读多写少推荐。

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

### 8.4 工厂模式与抽象工厂？

- **工厂方法**：父类定义创建接口，子类决定实例化哪个类（一个产品族的一个产品）。
- **抽象工厂**：创建**一族相关对象**（如 UI 主题的按钮+输入框+对话框）。
- .NET 落地：DI 注册 + `Func<string, IService>`（keyed services，.NET 8+）；简单工厂就是一个 `Create` 静态方法/类。
- 好处：解耦创建逻辑、便于替换实现。

### 8.5 策略模式（Strategy）？

- 定义一族算法，各自封装可互换，运行时选择。
- 例：支付策略（支付宝/微信/银行卡）、价格计算策略。
- .NET 落地：接口 + 多个实现 + DI 注册（`AddScoped<IPayment, Alipay>()` 或 keyed services）；比 `switch` 更好扩展（OCP）。

### 8.6 观察者模式（Observer）？与事件的关系？

- 一对多通知：被观察者状态变化时通知所有观察者。
- .NET 中**事件（event）就是观察者模式的实现**；还有 `IObservable<T>`/`IObserver<T>`（Rx）、`INotifyPropertyChanged`（WPF/MAUI 绑定）。
- 注意内存泄漏：订阅后忘记退订（事件源持有订阅者引用）。

### 8.7 装饰器模式（Decorator）？

- 动态给对象**添加职责**，包装原始对象，不修改原类。
- 例：日志装饰器、缓存装饰器、重试装饰器。
- .NET 落地：DI 支持装饰器（`TryAdd` + 手动包装，或 Scrutor）；`IEnumerable<T>` 注入可拿多个实现组合。
- 与继承对比：组合优于继承，更灵活。

### 8.8 适配器模式（Adapter）与外观模式（Facade）？

- **适配器**：把不兼容接口转换成客户端期望的接口（如旧系统 API 封装成新接口）。
- **外观**：为复杂子系统提供统一简化入口（如一个 `OrderFacade` 封装下单全流程）。
- 区别：适配器改变接口，外观简化接口；都用于解耦。

### 8.9 建造者模式（Builder）？

- 分步骤构造复杂对象，同一构建过程可产生不同表示。
- 例：`StringBuilder`（经典）、`WebApplicationBuilder`、配置 `Host.CreateDefaultBuilder`、Fluent 配置。
- 好处：参数多、可选参数多的对象构造可读性好。

### 8.10 责任链模式（Chain of Responsibility）？与中间件的关系？

- 请求沿链传递，每个处理器决定处理或传给下一个。
- **ASP.NET Core 中间件管道就是责任链模式的实现**；异常过滤器链、认证 Handler 也是。
- 好处：解耦发送者与接收者、灵活增减处理环节。

### 8.11 模板方法模式（Template Method）？

- 父类定义算法骨架，子类实现可变步骤。
- 例：`BackgroundService.ExecuteAsync`（骨架）+ 子类实现具体循环逻辑；`Stream`、`FileSystemWatcher` 等。
- 与策略的区别：模板方法是继承式（骨架在父类），策略是组合式（算法可替换）。

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

### 9.5 什么是 Outbox 模式？（高级）

- 解决"**本地事务提交与发消息不一致**"：把业务变更和 outbox 事件记录放在**同一数据库事务**；后台 Relay（轮询/CDC）读 outbox 表发消息，成功后标记已发送。
- 好处：不依赖分布式事务，保证最终一致；MassTransit、Npgsql/EF 有现成实现。

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

### 10.4 `Span<T>` / `ArrayPool<T>` 如何提升性能？（高级）

- `Span<T>`：零分配切片、栈上分配，避免中间字符串/数组拷贝（字符串解析、协议解析）。
- `ArrayPool<T>.Shared`：数组池化复用，避免高频大数组分配（ASP.NET Core 内部大量使用）。
- 注意：`ArrayPool` 借出的数组**必须归还**且不能长期持有；用 `Memory<T>`/`IMemoryOwner` 管理。

### 10.5 字符串拼接性能优化？

- 循环拼接用 `StringBuilder`；
- 高频插值用 `DefaultInterpolatedStringHandler`（编译器已优化）或 `String.Create`/`string.Concat` 精准合并；
- 避免 `+` 大循环、避免 `Format` 滥用；
- 用 `ReadOnlySpan<char>` 切片避免 `Substring` 分配。

### 10.6 `HttpClient` 的正确使用？（高频）

- **不要**每次 `new HttpClient()`（Socket 耗尽风险：TIME_WAIT 堆积）；**不要**长期复用而不管生命周期（DNS 变更不生效）。
- 用 **`IHttpClientFactory`**：自动管理 handler 生命周期（默认 2 分钟轮换）、命名/类型化客户端、集成 Polly 策略。
- 现代（.NET 8+）：`HttpClient` + `SocketsHttpHandler`（`PooledConnectionLifetime`）手动管理也可行。
- 注意：复用同一个 `HttpClient` 是线程安全的；BaseAddress + 相对路径；`Dispose` 由工厂管理。

### 10.7 异步代码常见性能坑？

- **sync-over-async**（`.Result`/`.Wait()`）：阻塞线程池线程 → 线程池饥饿 → 吞吐崩溃。
- **async void**：异常崩溃。
- 不必要 async：纯 CPU 短操作加 async 只是开销。
- 过度 `Task.Run`：把轻量同步代码丢线程池反而浪费。
- 并发 I/O 无节制：打满数据库/外部连接池（用 `SemaphoreSlim` 控制并发）。
- 每次都 `ConfigureAwait(false)` 不需要的场景过度使用。

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

### 11.3 什么是 AAA 模式？

- **Arrange**（准备数据/依赖）→ **Act**（执行被测代码）→ **Assert**（断言结果）。
- 每个测试只测一件事、命名清晰（`Method_Scenario_Expected`）。

### 11.4 什么是 Mock？Moq 怎么用？

- Mock：替身对象模拟依赖行为，隔离被测单元。
- Moq：`var mock = new Mock<IUserRepo>();` `mock.Setup(x => x.Get(1)).Returns(user);` `mock.Verify(x => x.Get(1), Times.Once);`
- 现代替代：NSubstitute、FakeItEasy；也鼓励用**手写 Fake**（简单、无魔法）。
- 陷阱：过度 Mock 导致测试与实现耦合（测试白盒化）；尽量 Mock 边界（I/O），少 Mock 内部逻辑。

### 11.5 什么是 `WebApplicationFactory<T>`？（高频）

- 集成测试入口：进程内启动完整 ASP.NET Core 应用（TestServer），真实管道 + 真实 DI，无需部署/端口。
- 能力：`ConfigureWebHost` 覆盖服务（替换 DB 为测试库/容器库）、`CreateClient()` 发真实 HTTP 请求。
- 用法：`IClassFixture<WebApplicationFactory<Program>>` 共享实例；配合 Testcontainers 起真实 Postgres/SQL Server。

### 11.6 什么是 Testcontainers？

- 测试中按需启动 Docker 容器（数据库、Redis、消息队列），用完销毁；集成测试环境与生产一致。
- 比内存数据库（EF InMemory/`SQLite InMemory`）更真实（约束、事务、并发行为一致）。

### 11.7 如何测试异步代码？

- `await` 被测方法；断言 `Task` 结果；异常用 `await Assert.ThrowsAsync<T>`。
- 避免在测试中 `.Wait()`/`.Result`；注意测试超时配置。

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
