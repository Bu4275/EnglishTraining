   M S B A M L   `   `   `  ����G  CEnglishTrain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:EnglishTrainEnglishTrain  5   	   (    "EnglishTrain.wordExplanationWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:EnglishTrain     5   	     � WindowStartupLocation$  CenterScreen=�62   
���   � � SizeToContent$ WidthAndHeight=�5   -    � Title$ wordExplanationWindow��5   	   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   .+�5   
   �� -   5        $��LB��5      $��0��6   
�������$��20(�6:   �� 5   
   $��1��6   
�������$!��新增此單字至資料庫��64   $��20(�6H   $��Center=�6V   � 5   
   $��2��6    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��* �6#   .+�5      �� -   5        $��0��5      $��20(�6%   $��_Yes是��63   6C   �� 5      -   5        $��1��5      $��20(�6%   $��_No否��63   6B   5      5      5      