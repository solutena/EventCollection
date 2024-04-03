# EventCollection

`Collection`에 변화가 있을 때 이벤트를 등록할 수 있는 클래스이다.

`serialize` 할 수 있다.



## Event

`Event`는 각 요소가 추가되거나 제거될 때 호출된다.

### Add, Remove

```
void Start()
{
  EventList<int> ints = new EventList<int>();
  ints.Event += (int item, bool isAdd) =>
  {
    if(isAdd)
      Debug.Log("Add : " + item);
    else
      Debug.Log("Remove : " + item);
  };

  ints.Add(0);
  ints.Remove(0);
}
```

결과 :

```
Add : 0
Remove : 0
```

## Changed

`Changed`는 `Event`가 종료된 후 한번만 호출된다.

```
void Start()
{
  EventList<int> ints = new EventList<int>();
  ints.Changed += () =>
  {
    Debug.Log("Changed");
  };

  ints.AddRange(new List<int> { 0, 1, 2, 3, 4 });
}
```

결과 : 

```
Changed
```
