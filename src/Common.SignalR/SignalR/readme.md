# Signalr的状态管理组件说明

## 组件概览

- EventBus : 事件通知和处理的总线
	- Hub 内部
	- HubContext 外部
	- HubCross 内外
	- SignalR 基类





- ClientMonitors： 客户端连接管理
	- 
	- 


## 关于IClientConnectionLocate

- ConnectionId Signalr的链接Id，刷新会变化，无法直接用来跟踪和维护业务上的状态
- ScopedId 业务上定义的范围（适用于多租户等场景）
- ClientId 业务上定义的一个客户端标识

IClientConnectionLocate: ScopedId + ClientId => ConnectionId => MyConnection(代表一个连接的业务数据)

## 关于ClientMethods

- ClientInvoke 客户端主动调用的方法
- ClientStub 客户端被动调用的方法

两种调用模式的解释：

- OnClientMethodCall 客户端 -> 中心 -> 客户端
- OnCallClientStub 外部（Api或服务） -> 中心 -> 客户端