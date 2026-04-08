using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QuizCinema
{
    public class DataCreator_Window : EditorWindow
    {
        [SerializeField] private Data data = new Data();
        private Vector2 _scrollPosition = Vector2.zero;     

        [Header("Resolution")]
        [SerializeField] private static float _resolutionWindowX = 510f;
        [SerializeField] private static float _resolutionWindowY = 344f;

        private SerializedObject serializedObject = null;
        private SerializedProperty questionProp = null;
        private string path;

        private void OnEnable()
        {
            serializedObject = new SerializedObject(this);
            Debug.Log(serializedObject);
            data.Questions = new Question[0];
            questionProp = serializedObject.FindProperty("data").FindPropertyRelative("Questions");
            Debug.Log(questionProp);
        }

        [MenuItem("Game/Data Creator")]
        public static void OpenWindow()
        {
            var window = GetWindow<DataCreator_Window>("Creator");

            window.minSize = new Vector2(_resolutionWindowX, _resolutionWindowY);
            window.Show();
        }

        private void OnGUI()
        {
            #region Header Section
            Rect headerRect = new Rect(25, 25, this.position.width - 50, 65);
            GUI.Box(headerRect, GUIContent.none);

            GUIStyle headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 26,
                alignment = TextAnchor.UpperLeft
            };
            headerRect.x += 10;
            headerRect.y += 10;

            GUI.Label(headerRect, "Data to XML Creator", headerStyle);

            Rect summaryRect = new Rect(headerRect.x, headerRect.y + headerRect.height - 30, headerRect.width, 15);
            GUI.Label(summaryRect, "Create the data that need to be included into XML file");
            #endregion

            #region Body Section
            Rect bodyRect = new Rect(25, (headerRect.y + headerRect.height) + 10, this.position.width - 50, this.position.height - headerRect.y - headerRect.height - 80);
            GUI.Box(bodyRect, GUIContent.none);

            var arraySize = data.Questions.Length;

            Rect viewRect = new Rect(bodyRect.x + 10, bodyRect.y + 10, bodyRect.width - 20, EditorGUI.GetPropertyHeight(questionProp));
            Rect scrollPosRect = new Rect(viewRect)
            {
                height = bodyRect.height - 20
            };
            _scrollPosition = GUI.BeginScrollView(scrollPosRect, _scrollPosition, viewRect, false, false,
                GUIStyle.none, GUI.skin.verticalScrollbar);
            var drawSlider = viewRect.height > scrollPosRect.height;

            Rect propertyRect = new Rect(bodyRect.x + 10, bodyRect.y + 10, bodyRect.width - (drawSlider ? 40: 20), 17);
            EditorGUI.PropertyField(propertyRect, questionProp, true);

            serializedObject.ApplyModifiedProperties();

            GUI.EndScrollView();
            #endregion

            #region Navigation

            Rect buttonRect = new Rect(bodyRect.x + bodyRect.width - 85, bodyRect.y + bodyRect.height + 15,
                85, 30);

            bool pressed = GUI.Button(buttonRect, "Create", EditorStyles.miniButtonRight); 
            
            if (pressed)
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = EditorUtility.SaveFilePanel("Save", "Asset", GameUtility.FileName, "xml");
                }

                Data.Write(data, path);
            }
          
            buttonRect.x -= buttonRect.width;
            pressed = GUI.Button(buttonRect, "Fetch", EditorStyles.miniButtonLeft);
            if (pressed)
            {
                path = EditorUtility.OpenFilePanel("Select", "Assets/StreamingAssets", "xml");
                if (!string.IsNullOrEmpty(path))
                {
                    var d = Data.Fetch(out bool result, path);
                    if (result)
                    {
                        data = d;
                    }

                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                }
            }
            /*
            buttonRect.x -= buttonRect.width * 3;
            pressed = GUI.Button(buttonRect, "CopyTextInfo", EditorStyles.miniButtonLeft);
            if (pressed)
            {
                var newData = data;
                var mas = newData.Questions;
                Debug.Log(mas.Length);
                for (int i = 0; i < mas.Length; i++)
                {
                    for (int j = 0; j < mas[i].Answers.Length; j++)
                    {
                        Debug.Log(mas[i].Answers[j].Info);
                        mas[i].Answers[j].TranslateInfo = mas[i].Answers[j].Info;
                    }
                }
                newData.Questions = mas;
                data = newData;
                Data.Write(data, path);

                Debug.Log(serializedObject.ToString());
                Debug.Log(questionProp.ToString());
            }
            */

            buttonRect.x -= buttonRect.width * 2;
            buttonRect.width = 100f;
            pressed = GUI.Button(buttonRect, "Questions", EditorStyles.miniButtonLeft);
            if (pressed)
            {
                var newData = data;
                var mas = newData.Questions;
                Debug.Log(mas.Length);
                for (int i = 0; i < mas.Length; i++)
                {
                    mas[i].ListInfoQuestion.Add(mas[i].Info);
                }
                newData.Questions = mas;
                data = newData;
                Data.Write(data, path);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            buttonRect.x -= buttonRect.width;
            pressed = GUI.Button(buttonRect, "NoteFilm", EditorStyles.miniButtonLeft);
            if (pressed)
            {
                var newData = data;
                var mas = newData.Questions;
                Debug.Log(mas.Length);
                for (int i = 0; i < mas.Length; i++)
                {
                    mas[i].ListNoteFilm.Add(mas[i].NoteFilm);
                }
                newData.Questions = mas;
                data = newData;
                Data.Write(data, path);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();


            buttonRect.x -= buttonRect.width;
            pressed = GUI.Button(buttonRect, "Answers", EditorStyles.miniButtonLeft);
            if (pressed)
            {
                var newData = data;
                var mas = newData.Questions;
                Debug.Log(mas.Length);
                for (int i = 0; i < mas.Length; i++)
                {
                    for (int j = 0; j < mas[i].Answers.Length; j++)
                    {
                        Debug.Log(mas[i].Answers[j].Info);
                        mas[i].Answers[j].InfoList.Add(mas[i].Answers[j].Info);
                        mas[i].Answers[j].InfoList.Add(mas[i].Answers[j].TranslateInfo);
                    }
                }
                newData.Questions = mas;
                data = newData;
                Data.Write(data, path);
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                #endregion
            }
        }
    }
}
