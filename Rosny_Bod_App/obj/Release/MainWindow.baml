   M S B A M L   `   `   `  ���� U  QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf   Rosny_Bod_App   Rosny_Bod_App.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     @d2http://schemas.microsoft.com/expression/blend/2008     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Dlvc4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf     -     � Title$  	Rosny bod��$��449.2��$	��800�� � Icon$ Rosnybod.ico��.��� .+��� 
�������$��1q� �� Padding$ 10q��� .+��  � ColumnDefinitions �� $��* ��� $��* ��� $��* ��� $��* ��� $��4* �.+��� -    ��Name$ Connect_to_device��$��	Připojit��$��0���� -   $ Disconnect_from_device��$��Odpojit��$��1��
���  ���� -    ��Name$ Status��$��4��$��	10,0,10,0q�$��
Neaktivní��
�������{�.   �� TextAlignment$ Center=��� -    ��Name$	 Tab��$��10,10,9.6,10q�$��	10,0,10,0q�$��Stretch=�$��Stretch=�$	��950��.@��� $��Automatické měření��.��� ���	#FFE5E5E5$��Stretch=�$��Stretch=� �� $��* ��� $��* �.+��� .+�� $��Stretch=�$��Stretch=�$��0��$	��300��$��5 10 0 0q� � RowDefinitions �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* � �� $��* ��� $��* ��� $��* ��� $��* �.+��� -   $ Auto_Messurement_Start��$��Spustit měření��$��0��$��0��$��Stretch=�$��Stretch=��� -   $ Auto_Messurement_Stop��$��Zastavit měření��$��Stretch=�$��Stretch=�$��0��$��2���� -   	 ��Name$	 CoolerTemp_auto��$��2��$��1��$��Stretch=�$��Stretch=�
 �� 
IsReadOnly
 . {�.   �� TextWrapping$
 Wrap=����� *Report.PT100TempSence+�� $��2��$��0��$��Teplota zrcadla °C��$
��Left=�$��Bottom=�$��9(��� $��Teplota okolí °C��$��2��$��2��$
��Left=�$��Bottom=�$��9(��� -	   $	 Envtemp_auto��$��Stretch=�$��2��$��3��
 . {�.  $
 Wrap=�$��Stretch=����� *Report.EnvTempReport+�� $��Atmosférický tlak hPa��$��4��$��2��$
��Left=�$��Bottom=�$	��8.9(��� -
   $	 Pressure_auto��$��5��$��2��
 . {�.  $
 Wrap=�$��Stretch=�$��Stretch=����� *Report.EnvPressureReport+�� $��Fotoresistor��$��6��$��2��$
��Left=�$��Bottom=�$��9(��� -   $	 
Light_auto��$��Stretch=�$��Stretch=�$��7��$��2��
 . {�.  $
 Wrap=����� *Report.LightSensorReport+�� $��Požadovaná teplota��$
��Left=�$��Bottom=�$��4��$��0��$��9(��� -   $	 Request_temperature_auto��$��Stretch=�$��Stretch=�$��5��$��0��$
 Wrap=�
 . {�.  ���� *RequestedString+�� $��1��.+��  �� $��25 ��� $	��300 ��� $��25 �.+��� $	��°C��$
��Left=�$��0��$	��Top=��� $��Vzorky��$��Right=�$��2��$	��Top=�#   LiveCharts.Wpf.CartesianChart  -     Name$ ch��$��1��&    LiveCharts.Wpf.Charts.Base.Chart   DisableAnimations .    Series �� *CollectionPT100_2+�� $��Výsledky měření��.��� $��5 5 5 5 q� �� $��* ��� $	��45* ��� $��* �.+��  �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+��� -   $ Add_row��$��0��$��Přidat řádek��$��0��$��5 0 5 0q��� -   $ 
Remove_row��$��0��$��Odstranit řádek��$��1��$�� 5 0 5 0q��� -   $ Export_list��$��0��$��
Exportovat��$��5��$�� 5 0 5 0q�&   System.Windows.Controls.DataGrid  -     Name$ Auto_Messuring_Results��$��1��   AutoGenerateColumns .  $��10,10,10,10q�$��10,10,10,10q���. $
��Auto=����� *Record+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$
 Čas��   Width5  /System.Windows.Controls.DataGridLengthConverter$	 160 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Time+ �� StringFormat$ {0:dd.MM.yy HH:mm:ss}��  $ Vnější teplota��$	 160  �� *ENV_Temperature+  $ Vnitřní teplota��$	 160  �� *PT100_Temperature+  $
 Tlak��$	 160  �� *ENV_Pressure+  $ Vlhkost��$	 160  �� *
Humidity+�� $��Manuální měření��.��� ���	#FFE5E5E5$��Stretch=� �� $��* ��� $��* �.+��� .+�� $	��300��$��Stretch=�$��Stretch=�$��5 10 0 0q� �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* � �� $��* ��� $��* ��� $��* ��� $��* �.+��� -   $ Manual_Messurement_Start��$��Spustit měření��$��Stretch=�$��Stretch=�$��0���� -   $ Manual_Messurement_Stop��$��Zastavit měření��$��Stretch=�$��Stretch=�$��2���� -   $	 
CoolerTemp��$��2��$��1��$��Stretch=�$��Stretch=�
 . {�.  $
 Wrap=����� *Report.PT100TempSence+�� $��2��$��0��$��Teplota zrcadla °C��$
��Left=�$��Bottom=�$��9(��� $��Teplota okolí °C��$��2��$��2��$
��Left=�$��Bottom=�$��9(��� -   $	 Envtemp��$��Stretch=�$��2��$��3��
 . {�.  $
 Wrap=�$��Stretch=����� *Report.EnvTempReport+�� $��Atmosférický tlak hPa��$��4��$��2��$
��Left=�$��Bottom=�$	��8.9(��� -   $	 Pressure��$��5��$��2��
 . {�.  $
 Wrap=�$��Stretch=�$��Stretch=����� *Report.EnvPressureReport+�� $��Fotoresistor��$��6��$��2��$
��Left=�$��Bottom=�$��9(��� -   $	 Light��$��Stretch=�$��Stretch=�$��7��$��2��
 . {�.  $
 Wrap=����� *Report.LightSensorReport+�� $��Zapsaná teplota��$��4��$��1��$
��Left=�$��Bottom=�$��9(��� -   $	 Set_Temperature��$��Stretch=�$��Stretch=�$��5��$��1��
 . {�.  $
 Wrap=�$��0 0 10 0q����� *RequestedValue+�� $��Požadovaná teplota��$
��Left=�$��Bottom=�$��4��$��0��$��9(��� -   $ 	 Request_temperature_manual��$��Stretch=�$��Stretch=�$��5��$��0��$
 Wrap=�$��0 0 10 0q����� *RequestedString+�� $��6��$��0��$��2��$��Stretch=�$��Stretch=�
�������$��0 1 10 1q��� -    ��Name$
 lbl1��$��6��$��0��$��2��$
��Left=�$��Stretch=�$��0 1 10 1q�$	��9.8(����� *RequestTemperatureManualText+�� -   $ 
ReqConfirm��$#��Potvrdit požadovanou teplotu��$��7��$��0��$��2��$��Stretch=�$��Stretch=�$��0 2 10 2q��� $��2��.+��  �� $��25 ��� $	��300 ��� $��25 �.+��� $	��°C��$
��Left=�$��0��$	��Top=��� $��Vzorky��$��Right=�$��2��$	��Top=�  -   $	 ch2��$��1�� .  �� *CollectionPT100+�� $��Softwarové konstanty��.��� ���	#FFE5E5E5$��12,10,7.8,10.4q� �� $��* ��� $	��15* � �� $��* ��� $
��1.8* �.+��� $S��MJsem si vědom, že změny v nastavení konstant mohou způsobit nefunkčnost��$��20 0 0 0q�$��1��$��0��$��Stretch=�$	��Top=�$��27���� $*��$zařízení a nebo jeho poškození.��$��20 0 0 0q�$��Stretch=�$��1��$��0��$��Bottom=�$��27���� -    ��Name$ 	Agreement��$��1��$��0��$�� ��$��Stretch=�$��Bottom=�$��	200 0 0 5q�� $��0��$��0�� �� $��* ��� $��2* ��� $��* �.+�G� -    G�Name$ Password_box��$��1��$��Stretch=�$��5 0 5 0q�$��Center=��� $��Heslo��$��0��$
��Left=�$��	10 2 10 2q�$��Center=��� -   $ Password_Confirm��$��Potvrdit��$��2��$��Right=�$��Center=�$��	10 2 10 2q�� $��1��$��0��$��2�� �� $��* ��� $��* ��� $��* ��� $��* � �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+��� $��0��$��0��$��
Regulátor��$��Center=�$	��Top=��� $��0��$��2��$��Automatické měření��$��Center=�$	��Top=�$	��132���� $��0��$��3��$$��Seznam sériových zařízení��$��Center=�$	��Top=��� $��1��$��0��$��r0��$
��Left=�$	��Top=�$��9(��� $��2��$��0��$��Td��$
��Left=�$	��Top=�$��9(��� $��3��$��0��$��Ti��$
��Left=�$	��Top=�$��9(��� -    $	 
r0_TextBox��$��1��$��0��$��Stretch=�$��23��$
 Wrap=�$��Bottom=�$��	30 0 30 0q�
 �� *NotReadableVariables+{��� *
PasswordOk+���� *HBridgeControl.r0+�� -!   $	 
Ti_TextBox��$��3��$��0��$��Stretch=�$��23��$
 Wrap=�$��Bottom=�$��	30 0 30 0q�
 �� *NotReadableVariables+{��� *
PasswordOk+���� *HBridgeControl.Ti+�� -"   $	 
Td_TextBox��$��2��$��0��$��Stretch=�$��23��$
 Wrap=�$��Bottom=�$��	30 0 30 0q�
 �� *NotReadableVariables+{��� *
PasswordOk+���� *HBridgeControl.Td+�� $��1��$��2��$��Hranice chlazení��$
��Left=�$	��Top=�$��9(��� -#   $	 Low_Light_TextBox��$��1��$��2��$��Stretch=�$��23��$
 Wrap=�$��Bottom=�$��	90 0 30 0q�
 �� *NotReadableVariables+{��� *
PasswordOk+���� *Automode.low_light_Const+�� $��2��$��2��$��Hranice zahřívání��$
��Left=�$	��Top=�$��9(��� -$   $	 High_Light_TextBox��$��2��$��2��$��Stretch=�$��23��$
 Wrap=�$��Bottom=�$��	90 0 30 0q�
 �� *NotReadableVariables+{��� *
PasswordOk+���� *Automode.high_light_Const+�� $��3��$��2��$��Zpoždění náběhu��$
��Left=�$	��Top=�$��9(��� -%   $	 speed_TextBox��$��3��$��2��$��Stretch=�$��23��$��	90 0 30 0q�$
 Wrap=�$��Bottom=�
 �� *NotReadableVariables+{��� *
PasswordOk+���� *Automode.speed_Const+�� $��4��$��2��$��Zpoždění měření��$
��Left=�$	��Top=�$��9(��� -&   $	 speed2_TextBox��$��4��$��2��$��Stretch=�$��23��$��	90 0 30 0q�$
 Wrap=�$��Bottom=�
 �� *NotReadableVariables+{��� *
PasswordOk+���� *Automode.speed2_Const+�� $��5��$��2��$��Počáteční krok °C��$
��Left=�$	��Top=�$��9(��� -'   $	 InicialStep_TextBox��$��5��$��2��$��Stretch=�$��23��$��	90 0 30 0q�$
 Wrap=�$��Bottom=�
 �� *NotReadableVariables+{��� *
PasswordOk+���� *Automode.inicialstep+�� $��6��$��2��$��Měřící krok °C��$
��Left=�$	��Top=�$��9(��� -(   $	 MessuringStep_TextBox��$��6��$��2��$��Stretch=�$��23��$��	90,0,30,0q�$
 Wrap=�$��Bottom=�
 �� *NotReadableVariables+{��� *
PasswordOk+���� *Automode.messuringstep+�� $��7��$��2��$��Teplota chladiče °C��$
��Left=�$	��Top=�$��9(��� $��7��$��2��
 . {�.  $��Stretch=�$��23��$��	90,0,30,0q�$
 Wrap=�$��Bottom=����� *Report.CoolerTemoSencetext+�� $��0��$��1��$%��Interní proměnné regulátoru��$��Center=�$	��Top=��� $��2��$��1��$��D��$
��Left=�$	��Top=�$��9(��� $��1��$��1��$��P��$
��Left=�$	��Top=�$��9(��� $��3��$��1��$��I��$
��Left=�$	��Top=�$��9(��� $��4��$��1��$��u��$
��Left=�$	��Top=�$��9(��� $��5��$��1��$��čas cyklu ms��$
��Left=�$	��Top=�$��9(��� $��1��$��1��{�.  
 . $��Stretch=�$��23��$
 Wrap=�$��Bottom=�$��	30 0 30 0q����� *HBridgeControl.SP+�� $��3��$��1��{�.  
 . $��Stretch=�$��23��$
 Wrap=�$��Bottom=�$��	30 0 30 0q����� *HBridgeControl.SI+�� $��2��$��1��{�.  
 . $��Stretch=�$��23��$
 Wrap=�$��Bottom=�$��	30 0 30 0q����� *HBridgeControl.SD+�� $��4��$��1��{�.  
 . $��Stretch=�$��23��$
 Wrap=�$��Bottom=�$��	30 0 30 0q����� *HBridgeControl.Su+�� $��5��$��1��{�.  
 . $��Stretch=�$��23��$
 Wrap=�$��Bottom=�$��	90,0,30,0q����� *HBridgeControl.STs+�� $��Proud A��$��5��$��3��$
��Left=�$	��Top=�$��9(��� -)   $ Debug��$��6��$��3��$��	Debug log��$��Stretch=�$��Bottom=�$��90 0 0 0q��� -*   $ Run_fan��$��6��$��1��$#��Spustit manualně ventilátor��$��Center=�$��Bottom=��� -+   $ Apply��$��7��$��3��$��Aplikovat změny��$��Right=�$��Bottom=��� $��Proud A��$��5��$��3��$
��Left=�$	��Top=�$��9(��� $��4��$��3��$��Stretch=�$��23��$
 Wrap=�$��Bottom=�$��	60,0,30,0q����� *ComPort.COM_Adress+�� $��COM port��$��4��$��3��$
��Left=�$	��Top=�$��9(��� $��5��$��3��{�.  
 . $��Stretch=�$��23��$
 Wrap=�$��Bottom=�$��	60,0,30,0q����� *AmperageShow+�� -,    ��Name$ 
SerialList��$��4��$��Stretch=�$��1��$��3��$��Stretch=�$��2 2 2 2q����� *ObservableDevices+