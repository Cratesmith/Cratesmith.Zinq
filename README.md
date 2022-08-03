# Zinq
Linq-like queries with zero allocations. 

Zinq is designed to be used as a Linq replacement in Unity3d projects (especially where projects contain many Linq queries that are affecting performance and re-writing them all is difficult).

## Usage 
For most containers, a Zinq query is almost identical to it's Linq counterpart but starts with `myCollection.Zinq()`  and end with a terminator function (most commonly `To()`)

### Simple example
Linq, returns a new List:
`list.Select(x=>5*x)` 

Zinq version, writes to outputList:
`list.Zinq().Select(_x => _x * 5).To(outputList);`

### Context queries (to avoid closures)
Often Linq-style queries require closures to pass in information from the calling function. 
````csharp
void GetAllItemsOfType(int _itemType, List<Item> _output)
{
    // _itemType is passed in as a closure allocation
    m_Items.Zinq().Select(x=>x.itemType==_itemType).To(_output);
}
````
Zinq provides "context" versions of queries where this information is passed in as a parameter to the lambda.
````csharp
void GetAllItemsOfType(int _itemType, List<Item> _output)
{
    // _itemType is the context for the query, and is passed in as the first parameter
    m_Items.Zinq().Select(_itemType, (_type,x)=>x.itemType==_type).To(_output);
}
````
The context invariant can be of any type. Which allows for multiple context parameters to be sent through as a value-tuple.
````csharp
void GetAllItemsOfType(int _itemType, int _itemCategory, List<Item> _output)
{
    // a context can contain multiple values by passing in an anonymous value-tuple
    m_Items.Zinq().Select((type:_itemType, category:_itemCategory), (c,x)=>x.itemType==c.type && x.itemCategory==c.category).To(_output);
}
````
## Supported Queries
* Where
* Select
* SelectMany
* Min/MaxOrDefault
* FirstOrDefault
* Any
* All
* To

## Installation

1. Open the unity package manager window
2. Click the add button and select "add from git url"
3. Paste in the following url and press enter: https://github.com/Cratesmith/Cratesmith.Zinq.git

## Performance
Detailed performance analysis still needs to be done to compare Zinq vs Linq vs hand written loops.

As a general rule Zinq queries have the same performance overhead as Linq, except there are no issues with performance from the garbage collector.

## How it works

### Avoiding creating new collections
Zinq doesn't create it's own collections, it just works on ones provided to it. Most notably the `To()` method which is the primary way to get the results of a query.

### Avoiding boxing

Zinq queries avoid allocations by performing operations on concrete typed enumerator structs, rather than accepting the IEnumerable interface like Linq does (which boxes the enumerators, creating lots of small allocations). 

To do this, Zinq queries are members of a struct called ZinqHelper which wraps a concrete type enumerator. The `Zinq()`call that starts the query is an extension method for collections which creates a ZinqHelper from that collection's enumerator.

Finally most queries are implemented as struct based enumerators, and the ZinqHelper calls return a new ZinqHelper struct on the stack which wraps the new query's enumerator, creating a chain from the current. 

### Avoiding closures
Zinq attempts to make the use of closures within it's queries unnecessary by providing "context" versions of each query so that the value can be passed to the lambda in a non-allocating manner.

## Work in progress

Zinq is a work in progress, below is a rough roadmap of planned things  to be done (not in particular order).

 - [ ] Change contexts to be refs
 - [ ] OrderBy/GroupBy queries
 - [ ] Performance comparisions (Linq, Hand written loops)


> Written with [StackEdit](https://stackedit.io/).
