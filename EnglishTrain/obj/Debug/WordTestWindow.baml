   M S B A M L   `   `   `  ����G  CEnglishTrain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:EnglishTrainEnglishTrain  5   	   !    EnglishTrain.WordTestWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:EnglishTrain     5   	   
���   �  � WindowStartupLocation$  CenterScreen=�5   -    � Title$ 單字訓練��5   	   $	��702��6   $
��1246��6#   .��5	      �  � RowDefinitions 5
   
   �� 5      $��2* �6   �� 5      $��6* �6   �� 5      $��2* �6   �� 5      $��* �6   �� 5      $��6* �6   .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      $��2* �6#   �� 5      $��* �6#   .+�5      �� -   5        $��0��5      
���   �
�������$��57(�6K   $��_Replay重播單字��6Y   6o   � 5      $��2��6    5      �� 5      $��* �6$   �� 5      $��* �6$   .+�5      	� -   5        $��GoogleRadioButton��5   +   $��0��6   $��_Google發音��6D   
�������$��28(�6k   $��Center=�6y    	� 	GroupName$ voiceRadioButton��6�    d� 	IsChecked$
 TrueY�6�   	� 5      -   5        $��YahooRadioButton��5   +   $��1��6   $��_Yahoo發音��6C   
�������$��28(�6i   $��Center=�6w   $ voiceRadioButton��6�   5      �� 5       -   5        $��_Back↶��5       $
��Left=�6&   $	��Top=�6A   $��37(�6Y   6g   5!      � 5"   
   $��1��6    5#      �� 5$      $��* �6#   �� 5%      $��* �6#   .+�5'      � $��0��6    5(      �� 5)      $��* �6$   �� 5*      $��* �6$   .+�5,      �� -   5        $��	wordLabel��5,   %   $��0��6   $y�Hidden=�66   
����·�$��57(�6_   $��Center=�6m   $��Center=�6�   �� 5-      -   5        $��
ruleLabel1��5-   %   $��0��6   $6��0請先聽聲音，在內心猜想是什麼單字��67   
��� ���$��37(�6f   $��Center=�6t   $��Center=�6�   �� 5.      -   5        $��phoneticSymbolLabel��5.   %   $��1��6   $y�Hidden=�6@   
���� �$��37(�6m   $��Center=�6{   $��Center=�6�   �� 5/      -   5        $��
ruleLabel2��5/   %   $��1��6   $��然後點選The next step��67   
��� ���$��37(�6g   $��Center=�6u   $��Center=�6�   50      �� 51      -	   5        $��chiMeanScrollViewer��51   +   $��1��6   $y�Hidden=�6F   .��52      �� -
   5        $��ChiTextBlock��52      
�������$��32(�6H   53      �� 54      -   5        $��ruleTextBlock1��54   (   $��1��6   
��� ���$y�Hidden=�6R   $��37(�6f   $��Center=�6t    �� TextAlignment$ Center=�6�   接著看英文單字�� 6�   猜想有哪些中文意思�� 6�   然後再按The next step�� 6�   ,*下面例句區可閉眼按0~n數字播放�� 6�   &$猜想該句子英文與中文意思6  55      � 56   
   $��2��6    57      �� 58      $��* �6#   �� 59      $��* �6#   �� 5:      $��* �6#   .+�5<      �� -   5        $��UnfamiliarButton��5<   %   $��0��6   $��_Unfamiliar��6=   $��38(�6S   
�������
���   �{�.  6�   �� 5=      -   5        $��MediumButton��5=   %   $��1��6   $��_Medium��69   $��38(�6K   
�������
���   �{�.  6�   �� 5>      -   5        $��FamiliarButton��5>   %   $��2��6   $��	_Familiar��6;   $��38(�6O   
�������
���   �{�.  6�   5?      �� 5@   
   -   5        $��
NextButton��5@      $��2��6   $��_The next step��60   $��45(�6I   
�������
���   �$y�Visible=�6}   6�   �� 5A   
   -   5        $��SlowVoiceCheckBox��5A       $��3��6   $��_Slow慢速播放例句��69   
�������$��28(�6b   $ FalseY�6p   $��Center=�6�   $
��Left=�6�   $��Center=�6�   {�. 6�   �� 5B   
   $��4��6   .��5C      � -   5        $��SentenceGrid��5C      5D      5E      5F      