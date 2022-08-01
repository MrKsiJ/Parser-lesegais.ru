***Здесь храниться сам парсер, для его работы требуется выполнить следующие требования:***

1.На компьютере должен быть установлен следующий программный софт:
* MySQL Installer
* MySQL Server
* MySQL Workbench
2.На компьютере должна быть выполнена настройка подключения в MySQL Workbench к MySQL Server.
![image](https://user-images.githubusercontent.com/57679050/182249097-35715c19-8195-439e-adb5-015c4c1b95cd.png)

 
В файле настроек парсера: Settings.ini должно быть корректно указан
*Адрес сервера базы данных
* Имя пользователя для подключения к базе данных
* Пароль пользователя
* По желанию можно изменить интервал проверки данных из источника
 ![image](https://user-images.githubusercontent.com/57679050/182249117-7f1890b5-e830-4bf8-ad84-c364aff3e4c0.png)

3.На MySQL Server должна существовать база данных с именем указаным в settings.ini в папке с утилитой в поле: databaseName.
 ![image](https://user-images.githubusercontent.com/57679050/182249131-59438b56-dd70-48d3-80de-87336ad0e5a5.png)

4.В базе данных должна существовать таблица, указанная в settings.ini в папке с утилитой в поле: dataBaseTable.
 ![image](https://user-images.githubusercontent.com/57679050/182249143-37f15224-8a1a-420a-b2cc-b3e2b351179b.png)

