   M S B A M L   `   `   `  ����G  CEnglishTrain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:EnglishTrainEnglishTrain  5   	   !    EnglishTrain.DatabaseWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:EnglishTrain     5   	   
���   �  � WindowStartupLocation$  CenterScreen=�5   -    � Title$ DatabaseWindow��5   	   $	��648��6    $
��1152��6-   .��5	      �  � RowDefinitions 5
   
   �� 5      $��2* �6   �� 5      $��8* �6   .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��4* �6#   .+�5      �  5      �� 5      $��* �6$   �� 5      $��* �6$   .+�5      �� $��0��6   $��40(�6%   $��Center=�63   $��Center=�6W   
���   �
�������$��搜尋單字��6�   �� 5      -   5        $��0��5      $��25(�6&   $
��Left=�64   $	��Top=�6O   
���   �
�������$��返回��6�   6�   �� 5      -   5        $��SearchTextBox��5   '   $��1��6   $��40(�6<   $��Center=�6J   $��Center=�6n   
���   �
�������6�   5      �� 5      -   5        $��1��5      $��50(�6%   $��刪除選取的單字��6N   
�������
���   �6s   5      � 5   
   $��1��6    5      �� 5       $��* �6#   �� 5!      $��4* �6#   .+�5#      �� -   5        $��WordListBox��5#      $��0��6)   $��27(�69   $��Center=�6G   $��Center=�6k   
���   �
�������6�   �� 5$      $��1��6   .��5%      �� -   5        $��explanationTextBlock��5%      
�������$��20(�6K   5&      �� 5'      $��1��6   .��5(      � -   5        $��dataGrid��5(      5)      5+      5,      5-      