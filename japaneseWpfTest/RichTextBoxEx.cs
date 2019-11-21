using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace japaneseWpfTest
{
    public class RichTextBoxEx : RichTextBox
    {
        public RichTextBoxEx() : base()
        {
            this.PreviewKeyDown += (s, e) =>
            {
                if ((Keyboard.IsKeyDown(Key.LeftShift)) && Keyboard.IsKeyDown(Key.Enter))
                {
                    var richTb = (s as RichTextBoxEx);

                    // 일본어 또는 중국어 입력시 생성되는 단어교환기? 팝업 (CompositionAdorner class)이 있을시
                    // shift + enter 기능을 방지한다.
                    if (richTb.CheckCompositionAdorner()) return;

                    // 현재 캐럿위치의 paragraph를 구함
                    var currentPara = richTb.CaretPosition.Paragraph;

                    // 선택되어진 text 제거
                    var startPointer = richTb.Selection.Start;
                    var endPointer = richTb.Selection.End;
                    richTb.Selection.Text = "";

                    // shift + enter시 다음줄로 내려야할 텍스트를 복사해 놓는다.
                    var tr = new TextRange(startPointer, currentPara.ContentEnd);
                    var selText = tr.Text;

                    tr.Text = "";
                    //Console.WriteLine(selText);

                    // 신규 paragraph 추가
                    var para = new Paragraph();
                    // 신규 마진 적용
                    para.Margin = new Thickness();

                    // shift + enter 시 아랫줄로 내려가야 할 텍스트가 있다면
                    // 신규 paragraph에 추가
                    if (!string.IsNullOrEmpty(selText))
                    {
                        para.Inlines.Add(selText);
                    }

                    // 현재 paragraph 뒤에 생성한 paragraph를 추가한다.
                    richTb.Document.Blocks.InsertAfter(currentPara, para);

                    // 캐럿을 다음 라인으로 이동해 준다.
                    richTb.CaretPosition = richTb.CaretPosition.GetLineStartPosition(1);

                    e.Handled = true;
                }
            };
        }

        public bool CheckCompositionAdorner()
        {
            try
            {
                var _scrollViewer = this.Template.FindName("PART_ContentHost", this) as ScrollViewer;

                if (_scrollViewer != null)
                {
                    var _scrollContentPresenter = FindChild<ScrollContentPresenter>(_scrollViewer);
                    if (_scrollContentPresenter != null)
                    {
                        var _tager = FindChild<Adorner>(_scrollContentPresenter, "CompositionAdorner");
                        if (_tager != null)
                        {
                            //var testlayer = AdornerLayer.GetAdornerLayer(_scrollViewer.Content as UIElement);
                            //testlayer.Remove(_tager);
                            //var layerList = testlayer.GetAdorners(_scrollViewer.Content as UIElement);                            
                            
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public T FindChild<T>(DependencyObject obj, string className = null) where T : DependencyObject
        {
            if (obj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    if (string.IsNullOrEmpty(className))
                    {
                        return (T)child;
                    }
                    else
                    {
                        if (((T)child).GetType().Name == className)
                        {
                            return (T)child;
                        }
                        else continue;
                    }
                    
                }
                else
                {
                    T childOfChild = FindChild<T>(child, className);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return null;
        }        
    }
}
