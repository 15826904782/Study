# Git 首次推送 GitHub 完整踩坑复盘文档（超详细·可复用排查手册）

## 一、本次操作背景

本地文件夹：包含 NodeJs、Study、中文目录文件

远程仓库：GitHub 新建仓库 Study\.git（带默认 README\.md）

操作目标：本地新建 Git 仓库，首次 push 到 GitHub 远程仓库

操作结果：历经四类经典报错，最终**SSH 443端口方案 \+ 合并远程分支**推送成功

---

## 二、报错1：fatal: 'origin' does not appear to be a git repository

### 1\. 报错现象

执行 git push 直接提示 origin 不是一个 git 仓库。

### 2\. 真实原因（核心）

本地虽然执行过 git init，但是**没有配置远程 origin 地址**。

Git 不知道要推送到哪个远程仓库，所以直接报错。

### 3\. 排查命令

```Plain Text
git remote -v
```

无输出 = 未配置远程仓库

### 4\. 正确修复命令

```Plain Text
git remote add origin 仓库地址
```

---

## 三、报错2：HTTPS 密码登录失效（GitHub 新版机制）

### 1\. 报错内容

remote: Invalid username or token\.

Password authentication is not supported for Git operations\.

### 2\. 原因详解（非常关键）

**GitHub 从 2021 年开始彻底禁用账号密码 HTTPS 推送**。

你输入 GitHub 登录密码一定会报错，必须使用：**Personal access token（PAT令牌）**

### 3\. 新手最大踩坑点

- 用网页登录密码 = 100% 失败

- 必须手动生成 ghp\_ 开头令牌

- 必须勾选 repo 权限

### 4\. 两种解决方式

方式1（HTTPS）：远程地址嵌入 token

方式2（最终本次成功方案）：**切换 SSH 协议**

---

## 四、报错3：SSH 22端口被封锁（Connection closed port 22）

### 1\. 报错内容

Connection closed by 198\.18\.0\.7 port 22

Could not read from remote repository\.

### 2\. 根本原因（90%国内用户遇到）

国内网络、校园网、公司网、运营商**默认屏蔽 GitHub SSH 22 端口**。

不是密钥错！不是权限错！是端口被拦截！

### 3\. 解决方案（本次最终采用）

让 SSH 不走 22 端口，走 443 端口（HTTPS 端口，永不被封）

### 4\. 配置步骤（永久生效）

在 \.ssh 目录新建 config 文件，内容：

```Plain Text
Host github.com
  Hostname ssh.github.com
  Port 443
  User git
```

### 5\. 验证成功

```Plain Text
ssh -T git@github.com
```

提示 yes 确认指纹 → 认证成功

---

## 五、报错4：push 被拒绝 fetch first（远程存在文件冲突）

### 1\. 报错内容

rejected master \-\> master \(fetch first\)

### 2\. 原因详细解释

你的 GitHub 远程仓库**新建时勾选了 README\.md**

导致：

- 远程仓库：有一次初始提交（README）

- 本地仓库：全新初始化、完全独立的提交记录

两个仓库历史完全不相关，Git 禁止直接覆盖推送。

### 3\. 正确解决命令（安全、不丢文件）

```Plain Text
git pull origin master --allow-unrelated-histories
```

作用：允许两个无关仓库历史合并

### 4\. 合并后再次推送

```Plain Text
git push -u origin master
```

---

## 六、本次最终成功日志

```Plain Text
Enumerating objects: 515, done.
Counting objects: 100% (515/515), done.
Compressing objects: 100% (294/294), done.
Writing objects: 100% (514/514), 55.44 MiB | 3.29 MiB/s, done.
Total 514 (delta 173), reused 511 (delta 172)
remote: Resolving deltas: 100% (173/173), done.
master -> master
branch 'master' set up to track 'origin/master'.
```

**含义：514个文件全部上传成功，本地与远程分支永久绑定**

---

## 七、完整正确流程（以后直接照这个走，零报错）

**首次上传 GitHub 标准无错流程**

```Plain Text
git init
git add .
git commit -m "init"
git remote add origin git@github.com:用户名/仓库名.git
git pull origin master --allow-unrelated-histories
git push -u origin master
```

---

## 八、四大终极避坑总结（本次所有问题根源）

1. **不要用 HTTPS 密码登录** → 必失败，用 Token 或 SSH

2. **国内网络不要用 SSH 22端口** → 必被封，必须改 443 端口

3. **远程仓库有 README 一定会冲突** → 必须 pull 合并

4. **push 报错先看 remote 配置** → 90% 是 origin 配置异常

---

## 九、后续日常使用简洁命令（已绑定分支）

```Plain Text
git add .
git commit -m "更新说明"
git push

git pull // 更新本地代码
```

> （注：部分内容可能由 AI 生成）
