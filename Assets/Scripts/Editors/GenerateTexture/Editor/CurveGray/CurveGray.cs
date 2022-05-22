using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zack.Editor
{
    /// <summary>
    /// 编辑器参数
    /// </summary>
    public class CurveGray : EditorWindow
    {
        [MenuItem("Editor/Texture/曲线灰度图", isValidateFunction:false)]
        static void OpenEditor()
        {
            EditorWindow.GetWindow<CurveGray>()._ExportPath = Application.dataPath + "/CurveGray.png";
            EditorWindow.GetWindow<CurveGray>().Show();
        }

        private AnimationCurve _Curve = new AnimationCurve();
        private int _Size;
        private string _ExportPath;
        void OnGUI()
        {
            EditorUtils.CreateIntField("尺寸:", ref _Size);
            EditorUtils.CreateCurveField("灰度曲线", ref _Curve);
            EditorUtils.CreateTextField("导出位置:", ref _ExportPath);
            EditorUtils.CreateButton("生成纹理", EditorParameters.k_ACButton, () =>
            {
                // 生成Texture2D
                var tex2D = new Texture2D(_Size, 1, TextureFormat.R8, false);
                tex2D.filterMode = FilterMode.Bilinear;
                for (int x = 0; x < _Size; ++x)
                {
                    float value = _Curve.Evaluate(x/(float)(_Size-1));
                    tex2D.SetPixel(x, 0, new Color(value, 0, 0, 0));
                }
                // 保存
                exportTexture(tex2D, _ExportPath);
                AssetDatabase.Refresh();
                Debug.Log("保存成功");
            });
        }
        
        void exportTexture(Texture2D texture, string path)
        {
            byte[] bytes = texture.EncodeToPNG();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream fs = new FileStream(path, FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }
        
        private void GenerateBayerDitherMatrix(ref float[] output, int n) {
            Debug.Assert(Mathf.IsPowerOfTwo(n) && n > 1);
            int log2N = Mathf.RoundToInt(Mathf.Log(n, 2));
            int[,] temp = new int[n, n];
            temp[0, 0] = 0;
            temp[0, 1] = 3;
            temp[1, 0] = 2;
            temp[1, 1] = 1;
            int currentSize = 2;
            for (int i = 1; i < log2N; i++) {
                for (int row = 0; row < currentSize; row++) {
                    for (int col = 0; col < currentSize; col++) {
                        temp[row, col + currentSize] = temp[row, col] * 4 + 3;
                    }
                }
                for (int row = 0; row < currentSize; row++) {
                    for (int col = 0; col < currentSize; col++) {
                        temp[row + currentSize, col] = temp[row, col] * 4 + 2;
                    }
                }
                for (int row = 0; row < currentSize; row++) {
                    for (int col = 0; col < currentSize; col++) {
                        temp[row + currentSize, col + currentSize] = temp[row, col] * 4 + 1;
                    }
                }
                for (int row = 0; row < currentSize; row++) {
                    for (int col = 0; col < currentSize; col++) {
                        temp[row, col] = temp[row, col] * 4;
                    }
                }

                currentSize *= 2;
            }
            for (int i = 0; i < n * n; i++) {
                output[i] = (1f + temp[i / n, i % n]) / (1 + n * n);
            }
        }
        
    }

}

