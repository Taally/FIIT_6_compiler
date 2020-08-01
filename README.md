# Проект по разработке оптимизирующего компилятора, ИММиКН им. Воровича, весна 2020
![CI](https://github.com/Taally/FIIT_6_compiler/workflows/CI/badge.svg)

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

**Визиторы и оптимизации по трёхадресному коду**
| Команда  | Задания | |3-адресный код
| :---------- |:-------:|:--:|:-:
| ИА | 1 \* ex, ex \* 1, ex / 1 => ex | if (true) st1; else st2; => st1 |Def-Use информация: накопление информации и удаление мёртвого кода на её основе
| Dedsec | a > a, a != a => false | if (false) st1; else st2; => st2 |Устранение переходов к переходам
| H6 | 2 == 4 => false | while (false) st; => null |Очистка кода от пустых операторов
| void | x = x => null | if (ex) null; else null; => null |Устранение переходов через переходы
| Пираты | 0 \* expr, expr \* 0 => 0 | 0 + expr => expr  |Учет алгебраических тождеств
| РМ | a == a, a >= a => true | if (true) st1; else st2; => st1 |Живые и мёртвые перем и удаление мёртвого кода (замена на пустой оператор)
| Письменский, Лутченко| 2 < 3 => true | if (false) st1; else st2; => st2 |Оптимизация общих подвыражений
| Потапов | 2 * 3 => 6 | a - a => 0 |Протяжка констант, Протяжка копий

**Все остальные задания**

[На доске](https://github.com/Taally/FIIT_6_compiler/projects/1)

---

[Ссылка на презентации](https://drive.google.com/drive/folders/127Dj3_lesQxzR_1TgBZtKZEX8gE-nLcQ?usp=sharing)

[Оценки за задания](https://docs.google.com/spreadsheets/d/18Ysxv_N48cqO2YVmVpBK5DG7GhJb2gEuApNWYwK9waI/edit?usp=sharing)

[Оценка вклада](https://docs.google.com/spreadsheets/d/1_VBAsqVdHGiMEaFM5YgWc6Kq4u-lkLW4V23hL-Z0UF4/edit?usp=sharing)

[Итоговые баллы](https://docs.google.com/spreadsheets/d/11ts0XgNcjWVp8IhpGTKyvTTQoF5XksyeygtxOV9F4dI/edit?usp=sharing)
