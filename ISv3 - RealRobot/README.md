Vi har lavet et program i Avalonia der forbinder til en UR-robot via TCP og lader den plukke varer fra tre lokationer (1/2/3) til en forsendelseskasse (S).
Brugeren kan trykke Connect for at oprette forbindelse og Process next for at starte plukningen.
Sådan bruges programmet:
Start UR-robot eller URSim og find robot-IP.
Indtast IP i feltet øverst og tryk Connect.
Når status viser Connected: True og Robotmode: RUNNING, tryk Process next.
Programmet bruger filen move-items-to-shipment-box.script til at styre robotten.
Koden er skrevet i C# og består af filerne App.axaml, MainWindow.axaml, Robot.cs, ItemSorterRobot.cs og DomainModels.cs.