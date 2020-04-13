# FIIT_6_compiler

|Название команды |Аббревиатура|Участники команды|
|:-----------------:|:-------------:|:-----------------:|
|Икеевские акулы|ИА|Татарова А., Шкуро Т.|
|Ростов-Москва|РМ|Володин Д., Моздоров Н.|
|void|void|Карякин В., Карякин Д.|
|Пираты|П|Рыженков С., Евсеенко А.|
|Dedsec|D|Галицкий К., Черкашин А.|
|Hot6|H6|Пацеев А., Ушаков И.|
||| Письменский М., Лутченко Д.|
||| Потапов И. |

## Задания

| Команда  | Задания | |
| :---------- |:-------:|:--:|
| ИА | 1 \* ex, ex \* 1, ex / 1 => ex | if (true) st1; else st2; => st1 |
| Dedsec | a > a, a != a => false | if (false) st1; else st2; => st2 |  
| H6 | 2 == 4 => false | while (false) st; => null |
| void | x = x => null | if (ex) null; else null; => null |
| Пираты | 0 \* expr, expr \* 0 => 0 | 0 + expr => expr  |
| РМ | a == a, a >= a => true | if (true) st1; else st2; => st1 |
| Письменский, Лутченко| 2 < 3 => true | if (false) st1; else st2; => st2 |
| Потапов | 2 * 3 => 6 | a - a => 0 |