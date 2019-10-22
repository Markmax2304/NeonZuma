# NeonZuma documentation
This NeonZuma documentation of project based on Entitas framework. It looks like list of components and systems with its short description

## Context
- [Globals](#Globals)
- [Level](#Level)
- [Logic](#Logic)
    - [Ability](#Ability)
    - [Animation](#Animation)
    - [Balls](#Balls)
    - [Chain](#Chain)
    - [Collision](#Collision)
    - [GameManagement](#GameManagement)
    - [Input](#Input)
    - [Path](#Path)
    - [Player](#Player)
    - [Projectile](#Projectile)
    - [Score](#Score)
- [Utils](#Utils)
___
## Globals
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***DebugAccess***|Global|:white_check_mark:||:triangular_flag_on_post:|Разрешение на запись в лог<br/>Берётся из настроек<br/>|
___
[:arrow_up:Context](#Context)
## Level
|System|Types|Description|
|------|:-----|:-----------|
|***ExecuteLevelLogic***|Execute|Логика выполнения систем, что представляют игровую логику уровня<br/>|
___
|Reactive System|Types|Entity|Triggers|Description|
|---------------|:-----|:------:|:--------|:-----------|
|***FinishLevel***|:x:|Manage|FinishLevel|Логика окончания игрового уровня и его выгрузка, а также вызов всех событий подписаных на это<br/>|
|***UploadLevel***|:x:|Manage|StartLevel|Логика начала игрового уровня и загрузка его, а также вызов всех событий подписаных на это<br/>|
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***FinishLevel***|Manage|:white_check_mark:|:white_check_mark:|:triangular_flag_on_post:|Флаг о том, что левел завершён и будет выгружен<br/>|
|***LevelPlay***|Manage|:white_check_mark:|:white_check_mark:|:triangular_flag_on_post:|Флаг того, что игровой уровень проигрывается, если его отключить, то и проигрывание систем уровня приостановится<br/>|
|***LogicSystems***|Manage|:white_check_mark:||Systems|Компонент хранит системы, что выполняются на игровом уровне<br/>В принципе, этот компонент и есть игровой уровень, в какой-то степени<br/>|
|***StartLevel***|Manage|:white_check_mark:|:white_check_mark:|:triangular_flag_on_post:|Флаг о том, что левел начался и будет загружен<br/>|
___
[:arrow_up:Context](#Context)
## Logic
[:arrow_up:Context](#Context)
### Ability
|System|Types|Description|
|------|:-----|:-----------|
|***InputAbility***|Execute|Тестовая логика создания флагов абилок по нажатию на нумерные клавиши<br/>|
|***UpdatePointerLength***|<ul><li>Execute</li><li>Initialize</li><li>TearDown</li></ul>|Логика обработки и просчёта указки, когда она активна<br/>Длина указки равняется расстоянию до ближайшего шара в направлении выстрела или до конца экрана, если шаров нет на пути<br/>|
___
|Reactive System|Types|Entity|Triggers|Description|
|---------------|:-----|:------:|:--------|:-----------|
|***ExplodeBall***|:x:|Input|Collision|Логика взрыва взрывного шара, после его столкновения с другими шарами в цепи<br/>Уничтожаются все шары в заданом радиусе<br/>|
|***InvokingAbility***|<ul><li>Initialize</li><li>TearDown</li></ul>|Input|AbilityInput|Логика реагирования на флаги абилок и запуска соответствующих обрабатывающий процессов<br/>|
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***AbilityInput***|Input|||TypeAbility|Тип абилки, которую надо вызвать<br/>|
|***Explosion***|Game|||:triangular_flag_on_post:|Флаг о том, что данный снаряд является взрывным и после столкновения с цепью шаров произойдёт взрыв<br/>|
|***ExplosionCount***|Global|:white_check_mark:||int|Количество доступных взрывов<br/>Инкрементится после установки флага взрыва<br/>Является своеобразным количество последующих выстреленных шаров, которые будут взрывными<br/>|
|***Freeze***|Global|:white_check_mark:||:triangular_flag_on_post:|Флаг заморозки, который запускает процесс заморозки<br/>|
|***Pointer***|Global|:white_check_mark:||:triangular_flag_on_post:|Флаг указки, который запускает процесс отображения указки<br/>|
|***Rollback***|Global|:white_check_mark:||:triangular_flag_on_post:|Флаг отката, который запускает процесс отката<br/>|
___
[:arrow_up:Context](#Context)
### Animation
|Reactive System|Types|Entity|Triggers|Description|
|---------------|:-----|:------:|:--------|:-----------|
|***FinishMoveAnimation***|TearDown|Game|AnimationDone|Логика обработки окончания анимации и вызова пост событий<br/>|
|***MoveAnimationControl***|TearDown|Game|MoveAnimation|Логика запуска и прерывания анимаций движения объектов<br/>|
|***ScaleAnimationControl***|TearDown|Game|ScaleAnimation|Логика запуска и прерывания анимаций масштабирования объектов<br/>|
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***MoveAnimation***|Game|||<ul><li>float</li><li>Vector3</li><li>Action</li></ul>|Данные для запуска анимации движения у объекта<br/>Также есть пост событие, которое выставляет флаг об окончании анимации<br/>|
|***AnimationDone***|Game|||:triangular_flag_on_post:|Флаг о том, что анимация окончена<br/>Нужен, дабы была возможность отловить окончание анимации в определённый нужный момент<br/>|
|***AnimationInfo***|Game|||List<Action>|Список событий которые должны быть обработаны по завершению анимации<br/>Если анимация прерывается другой анимацией, то новые пост события идут в этот компонент<br/>|
|***ScaleAnimation***|Game|||<ul><li>float</li><li>float</li><li>Action</li></ul>|Данные для запуска анимации масштабирования объекта<br/>Также есть пост событие, которое выставляет флаг об окончании анимации<br/>|
___
[:arrow_up:Context](#Context)
### Balls
|System|Types|Description|
|------|:-----|:-----------|
|***CheckAndSpawnBall***|<ul><li>Execute</li><li>Initialize</li><li>TearDown</li></ul>|Логика проверки готовности к появлению нового шара, а также самого создания новго шара<br/>|
|***UpdateBallDistanceBySpeed***|<ul><li>Execute</li><li>Initialize</li><li>TearDown</li></ul>|Логика обновления позиции шаров относительно скорости. То есть, своего рода, логика движения шаров по треку<br/>|
___
|Reactive System|Types|Entity|Triggers|Description|
|---------------|:-----|:------:|:--------|:-----------|
|***ChangeBallPositionOnPath***|TearDown|Game|DistanceBall|Логика изменения позиции шаров на треке, в зависимости от их дистанции<br/>|
|***CountBallColors***|:x:|Game|AnyOf:<br/><ul><li>AddedBall</li><li>RemovedBall</li></ul>|Система подсчёта количества шаров каждого цвета на игровой зоне<br/>|
|***UpdateColorBall***|:x:|Game|AllOf:<br/><ul><li>Color</li><li>Sprite</li></ul>|Логика обновления цвета шара на спрайте<br/>|
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***AddedBall***|Game|||:triangular_flag_on_post:|Флаг о том, что шар появился в игровой зоне<br/>|
|***BackEdge***|Game|||:triangular_flag_on_post:|Флаг о том, что шар является задней кромкой цепи<br/>|
|***BallColors***|Global|:white_check_mark:||Dictionary<ColorBall, int>|Количество шаров каждого цвета, что сейчас существуют на уровне<br/>Ипользуется для рандомизатора цвета шара у жабы<br/>|
|***BallId***|Game|||int|Идентификатор шара для упрощённого поиска и идентификации<br/>|
|***CheckTargetBall***|Game|||:triangular_flag_on_post:|Флаг о том, что этот шар был только что добавлен в цепь и следует проверить, можно ли уничтожить подобные шары рядом<br/>|
|***Color***|Game|||ColorBall|Цвет шара<br/>|
|***DistanceBall***|Game|||float|Дистанция, что была пройдена шаром от начала трека<br/>|
|***FrontEdge***|Game|||:triangular_flag_on_post:|Флаг о том, что шар является переднеё кромкой цепи<br/>|
|***GroupDestroy***|Game|||int|Номер для группирования шаров при их уничтожении, чтобы можно было объединить несколько шаров в одну группу<br/>|
|***GroupSpawn***|Game|||int|Количество шаров, которое должно быть заспавнено одновременно<br/>|
|***RemovedBall***|Game|||:triangular_flag_on_post:|Флаг о том, что шар исчез из игровой зоны игрока<br/>|
___
[:arrow_up:Context](#Context)
### Chain
|Reactive System|Types|Entity|Triggers|Description|
|---------------|:-----|:------:|:--------|:-----------|
|***CutChain***|Initialize|Game|Cut|Логика проверки цепи на разрыв и непосредственно самого разрыва<br/>|
|***MatchInsertedBallInChain***|:x:|Game|CheckTargetBall|Логика расчёта количества соседствующих шаров со вставленным такого же цвета.<br/>А также определение возможно ли уничтожения шаров одного цвета<br/>Есть логика инициализации очков за уничтожение<br/>|
|***SetChainEdges***|:x:|Game|ResetChainEdges|Логика обновления кромок цепей<br/>|
|***SetChainSpeed***|<ul><li>Initialize</li><li>Cleanup</li><li>TearDown</li></ul>|Game|UpdateSpeed|Логика обновления скоростей цепей на треке<br/>|
|***VisualDestroyingBalls***|<ul><li>Initialize</li><li>Cleanup</li></ul>|Game|GroupDestroy|Визуальное уничтожение шаров по группам<br/>|
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***ChainId***|Game|||int|Идентификатор цепи. Предназначен для упрощённого поиска цепи под ID<br/>|
|***ChainSpeed***|Game|||float|Скорость конкретной цепи. Эта скорость влияет на движения шаров цепи<br/>|
|***Cut***|Game|||:triangular_flag_on_post:|Флаг о том, что данную цепь стоит проверить на разрывы<br/>|
|***GravitateCombo***|Game|||int|Количество сработаных подряд притягиваний на данной цепи.<br/>Предназначен для расчёта комбо, а также модификатора скорости притяжения<br/>|
|***ParentTrackId***|Game|||int|Идентификатор родительского трека цепи для упрощённой связки цепи с треком<br/>|
___
[:arrow_up:Context](#Context)
### Collision
|System|Types|Description|
|------|:-----|:-----------|
|***BallOverlap***|<ul><li>Execute</li><li>Initialize</li></ul>|Логика определения Overlap коллизий<br/>|
|***BallRayCast***|<ul><li>Execute</li><li>Initialize</li></ul>|Логика определения RayCast коллизий<br/>|
___
|Reactive System|Types|Entity|Triggers|Description|
|---------------|:-----|:------:|:--------|:-----------|
|***CollidingAndInsertingProjectile***|Initialize|Input|Collision|Логика просчёта столкновения снаряда с цепью шаров, а также логика вставки шара в цепь<br/>|
|***CollisionObjectDestroy***|TearDown|Input|Collision|Логика уничтожения или отключения шаров, если они выходят за пределы игрового экрана<br/>|
|***ConnectChains***|Initialize|Input|Collision|Логика соединения цепей шаров после их столкновения<br/>|
|***EnteringBallsToScreen***|:x:|Input|Collision|Логика вхождения шара на игровой экран<br/>|
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***Collision***|Input|||<ul><li>TypeCollision</li><li>GameEntity</li><li>GameEntity</li></ul>|Данные о случившейся коллизии, а именно тип и два объекта учавствующих в столкновении<br/>|
|***Overlap***|Game|||:triangular_flag_on_post:|Флаг о том, что эта сущность учавствует в просчёте столкновений по Overlap методу<br/>|
|***RayCast***|Game|||Vector3|Флаг о том, что эта сущность учавствует в просчёте столкновений по RayCast методу<br/>Также есть данные о последнем местоположении для расчёта вектора движения<br/>|
___
[:arrow_up:Context](#Context)
### GameManagement
|System|Types|Description|
|------|:-----|:-----------|
|***TickCounters***|<ul><li>Execute</li><li>TearDown</li></ul>|Логика которая клокает счётчик<br/>|
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***Counter***|Game|||<ul><li>float</li><li>Action</li></ul>|Счётчик времени. Имеет также событие которое должно отработать по окончанию счётчика<br/>|
|***StartPlayEvent***|Manage|:white_check_mark:||List<Action>|Список событий, которые выполняются на старте игрового уровня<br/>Предназначен для спауна кучи шаров в начале уровня<br/>|
___
[:arrow_up:Context](#Context)
### Input
|System|Types|Description|
|------|:-----|:-----------|
|***TouchHandle***|<ul><li>Execute</li><li>TearDown</li></ul>|Логика отлова и инициализции тач ввода в игре<br/>|
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***TouchPosition***|Input|||Vector2|Позиция на игровом поле, где произошёл touch<br/>|
|***TouchType***|Input|||TypeTouch|Тип действия, которое будет вызвано тачем<br/>|
___
[:arrow_up:Context](#Context)
### Path
|System|Types|Description|
|------|:-----|:-----------|
|***InitializePath***|<ul><li>Initialize</li><li>TearDown</li></ul>|Логика начальной инициализции треков<br/>|
___
|Reactive System|Types|Entity|Triggers|Description|
|---------------|:-----|:------:|:--------|:-----------|
|***GameEndProcess***|:x:|Global|BallReachedEnd||
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***BallReachedEnd***|Global|:white_check_mark:||:triangular_flag_on_post:||
|***CurrentNormalSpeed***|Global|:white_check_mark:||float|Текущая нормальная скорость на уровне<br/>Выделен отдельный компонент, чтобы можно было легко менять и не требовалось запоминать в отдельную переменную<br/>|
|***NearToEnd***|Game|||:triangular_flag_on_post:|Флаг о том, что шары на данном треке очень близко к концу и следует уменьшить скорость<br/>|
|***PathCreator***|Game|||PathCreator|Класс хранящий путь и связаную с ним логику<br/>|
|***Randomizer***|Game|||Randomizer|Класс хранящий логику рандомизатора шаров<br/>|
|***SpawnAccess***|Game|||:triangular_flag_on_post:|Флаг о том, что сейчас можно создавать новые шары<br/>|
|***TrackId***|Game|||int|Идентификатор трека для упрощённого поиска и связки с цепями<br/>|
|***TrackStorage***|Game|:white_check_mark:||TrackStorage||
|***UpdateSpeed***|Game|||:triangular_flag_on_post:|Флаг о том, что следует обновить скорости цепей на данном треке<br/>|
___
[:arrow_up:Context](#Context)
### Player
|System|Types|Description|
|------|:-----|:-----------|
|***InitializePlayer***|<ul><li>Initialize</li><li>TearDown</li></ul>|Логика инициализации игрока<br/>|
___
|Reactive System|Types|Entity|Triggers|Description|
|---------------|:-----|:------:|:--------|:-----------|
|***BallExchangePlayer***|:x:|Input|AllOf:<br/>TouchType|Логика смены текущего снаряда на следующий снаряд. То есть, свап снарядов местами<br/>|
|***RotatePlayer***|:x:|Input|AllOf:<br/><ul><li>TouchPosition</li><li>TouchType</li></ul>|Логика вращения игрока относительно того, куда направлен тач<br/>|
|***ShootPlayer***|<ul><li>Initialize</li><li>TearDown</li></ul>|Input|AllOf:<br/><ul><li>TouchPosition</li><li>TouchType</li></ul>|Логика выстрела снаряда и последующей перезарядки, а также создания нового шара для перезарядки<br/>|
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***FireAccess***|Global|:white_check_mark:||:triangular_flag_on_post:|Флаг о том, что в данный момент игрок может сделать выстрел<br/>|
|***Player***|Game|:white_check_mark:||:triangular_flag_on_post:|Флаг о том, что данная сущность является игроком<br/>|
|***Recharge***|Game|:white_check_mark:||:triangular_flag_on_post:|Флаг о том, что данная сущность является следующий снарядом<br/>То есть, снарядом для перезарядки<br/>|
|***RechargeDistance***|Global|:white_check_mark:||Vector3|Дистанция от центра игрока до положения снаряда<br/>|
|***Shoot***|Game|:white_check_mark:||:triangular_flag_on_post:|Флаг о том, что данная сущность является текущим снарядом<br/>|
___
[:arrow_up:Context](#Context)
### Projectile
|System|Types|Description|
|------|:-----|:-----------|
|***ShootingForce***|<ul><li>Execute</li><li>Initialize</li></ul>|Логика движения снаряда во время полёта<br/>|
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***Force***|Game|||Vector2|Направление в котором должен лететь снаряд<br/>|
|***ForceSpeed***|Global|:white_check_mark:||float|Скорость с которой должен лететь снаряд<br/>|
|***Projectile***|Game|||:triangular_flag_on_post:|Флаг о том, что данная сущность является снарядом<br/>|
___
[:arrow_up:Context](#Context)
### Score
|Reactive System|Types|Entity|Triggers|Description|
|---------------|:-----|:------:|:--------|:-----------|
|***ScoreCounter***|<ul><li>Initialize</li><li>TearDown</li></ul>|Manage|ScorePiece|Логика подсчёта очков на игровом уровне<br/>|
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***MoveBackCombo***|Manage|:white_check_mark:||int|Комбо откатов цепей назада подряд<br/>|
|***ScorePiece***|Manage|||int|Данные о том, сколько очков нужно прибавить к общему счётчику<br/>|
|***ShootInRowCombo***|Manage|:white_check_mark:||<ul><li>int</li><li>bool</li></ul>|Комбо уничтожений шаров выстрелом шара подряд<br/>Также есть флаг о том, что это уничтожение было именно шаром. Однако, его стоит вынести в отдельный компонент<br/>|
|***TotalScore***|Manage|:white_check_mark:||int|Общий счётчик очков на уровне<br/>|
___
[:arrow_up:Context](#Context)
## Utils
|System|Types|Description|
|------|:-----|:-----------|
|***UpdateDeltaTime***|Execute|Логика обновления клока относительно Time.deltaTime<br/>|
___
|Reactive System|Types|Entity|Triggers|Description|
|---------------|:-----|:------:|:--------|:-----------|
|***DestroyGameEntityHandle***|:x:|Game|Destroyed|Логика уничтожения GameEntity<br/>|
|***DestroyInputEntityHandle***|:x:|Input|Destroyed|Логика уничтожения InputEntity<br/>|
|***DestroyManageEntityHandle***|:x:|Manage|Destroyed|Логика уничтожения ManageEntity<br/>|
|***RecordLogMessage***|:x:|Manage|LogMessage|Логика записи сообщений в лог<br/>|
___
|Component|Contexts|Unique|Event|Fields|Description|
|---------|:--------|:------:|:-----:|:------|:-----------|
|***DeltaTime***|Global|:white_check_mark:||float|Представляет кешированую deltaTime<br/>На протяжении одного цикла выполнения всех систем, Time.deltaTime может меняться, что приведёт к неверным расчётам<br/>|
|***Destroyed***|<ul><li>Game</li><li>Input</li><li>Manage</li></ul>|||:triangular_flag_on_post:|Флаг о том, что данную сущность следует уничтожить<br/>|
|***LineRenderer***|Game|||LineRenderer|Хранит Line Renderer компонент из Юнити<br/>|
|***LogMessage***|Manage|||<ul><li>string</li><li>TypeLogMessage</li><li>bool</li><li>Type</li></ul>|Данные о сообщение, которое будет записано в Лог<br/>Имеет само сообщение, тип записи, следует ли писать в Юнити лог тоже, а также источник вызова лога<br/>|
|***Sprite***|Game|||SpriteRenderer|Спрайт шара<br/>|
|***SpriteGlowEffect***|Game|||SpriteGlowEffect||
|***Transform***|Game|||Transform|Трансформ компонент из Юнити<br/>|
___
[:arrow_up:Context](#Context)
