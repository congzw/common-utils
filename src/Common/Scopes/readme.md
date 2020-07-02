# 范围上下文的一些说明

## ScopeContext

- 代表某个逻辑上的范围划分，可以适用于多租户等场景
- ScopeId是范围的键，范围内的所有变量存于Items字典中
- GetScopeContext用于获取上下文，默认实现可以按需要被替换（可重置Resolve，通过挂接新的IScopeService实例来实现）

## IScopeService

- 范围上下文的持久化相关服务，用于完成范围上下文的获取，保存，移除，清空等
- 基于接口，并自带默认内存实现（单例使用），支持按需扩展
