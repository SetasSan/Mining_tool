   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem   DatabaseAnalizer!   DatabaseAnalizer.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     9System&clr-namespace:System;assembly=mscorlib       � Title$  
MainWindow��$	��812��$��1331.625��.��� .+�o� -    o�Name$ meniu1��$��26��$	��Top=�$��0,0,10,0q�.@�m� -    m�Name$ menuFile��$��_File��.@�m� $��	Select DB��m� -   $��Export log file��m� -   $
��Exit��m� $��_View��m� $��_About��� $	��100��$	��100���� -    ��Name$ data_displayer��$��220,0,332,40q� �� TextWrapping$
 Wrap=�$�� ��$	��170��$��Bottom=�$
��Auto=�c� $��28��$��	0,31,10,0q�$	��Top=�.@��� $��70��|�.  {�.  $��Select DB :���� -    ��Name$ Databases_list��$��23��$��0q�$	��Top=�$	��120���� -   $��Analyze��$��23��$	��Top=�$��75���� -   $��Find relations��$��23��$	��Top=�$��80�� � ToolTip$  Find relations from schema���� -	   $��Filter��$��23��$	��Top=�$��80���� -
   $��Generate Log��$��23��$	��Top=�$��80���� $��
10,64,0,45q�$
��Auto=�$
��Left=�$	��200��.���� -    ��Name$ ButtonsPanel��$
��Left=�$	��200���� -    ��Name$ tab_main��$��220,59,332,210q�.@��� -   	 ��Name$	 tab_table_data��$��
Table data��.��� ���	#FFE5E5E5$��0,0,1,0q�.+�&   System.Windows.Controls.DataGrid  -   
  Name$
 
table_data���� $��Database tables relations��.��� ���	#FFE5E5E5$��
0,-2,-2,-5q� � ContextMenu �� .+��� $
��Auto=�$
��Auto=�$��0,0,0,27q�.���� -   $��Relation_Canvas�� J� 	AllowDrop . 
������ $
��3000��$
��3000��a�  �� Left$ 0��
 �� Top$	 459��$��Bottom=� �� Orientation$ 
Horizontal=�.+��� -   $��Generate��$
��Left=�$	��100���� $	��300��.��64Tip: Mark table id and drag line till relation field�� -   $	 tab_generated_table_data��$��Generated data��.��� ���	#FFE5E5E5$��0,0,1,0q�.+�  -   $
 generated_table_data���� -   $	 tab_generated_log��$��Generated Log��.��� ���	#FFE5E5E5$��0,0,1,0q�.+��� -   $ geberated_table_log���� $��
0,86,10,40q�$��Right=�$	��317��.@��� $��Table parametres��.��� ���	#FFE5E5E5$��Right=�$	��300��.+�  -   $
 Table_parametres��$��	-14,0,0,0q�$��Disabled=�$��Right=�$	��314���� $��Table settings��.��� -    �Name$ Table_settings�� � ColumnDefinitions �� $	��68* ��� $
��239* �.+�  $��5q�   AutoGenerateColumns .  $��2������    Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$
 Name��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Name+4  .System.Windows.Controls.DataGridTemplateColumn  $ Process Type��   Width5  /System.Windows.Controls.DataGridLengthConverter$ *    CellTemplate �� .R��� -   $��2q� �� *Name+���� *Types+�� $��220,0,332,0q�$��40��$��Bottom=�$ 
Horizontal=�.+�� -    �Name$ Loader��$��26��$��200px�� J� OpacityMask
 �   �$
��Left=�$��Bottom=��� -   $	��140��$��0,14,0,0q�$��Cancell���� -    ��Name$ 
tbProgress��$�� ��$
��Left=�$	��141��$��0,10,0,0q�