// using System.Collections;
// using System.Collections.Generic;
// using System.Xml.Schema;
// using TMPro;
// using UnityEngine;

// public class StageManager : MonoBehaviour
// {
//     int[] dx = new int[4] { 0, 0, -1, 1 };
//     int[] dy = new int[4] { 1, -1, 0, 0 };

//     // 맵의 가로, 세로 
//     public int x;
//     public int y;

//     public int xx;
//     public int yy;

//     // 찍어낼 방의 수와 현재 찍어낸 방의 수
//     public int count;
//     public int current_count;

//     // 방의 개수 및 타입을 행렬로 표현
//     public int[,] stage;

//     // x,y
//     public int[,] InitStage()
//     {
//         int[,] array = new int[x, y];
//         return array;
//     }

//     public void RandomStage()
//     {
//         int start_x = Random.Range(0, x);
//         int start_y = Random.Range(0, y);

//         stage[start_x, start_y] = 1;

//         int nx = start_x;
//         int ny = start_y;

//         current_count = 0;
//         while (current_count < count)
//         {
//             int tx = nx;
//             int ty = ny;

//             nx = Random.Range(tx - 1, tx + 2);
//             ny = Random.Range(ty - 1, ty + 2);

//             if (0 <= nx && nx < x && 0 <= ny && ny < y)
//             {
//                 if (stage[nx, ny] == 0)
//                 {
//                     stage[nx, ny] = 1;
//                     current_count++;
//                 }
//                 else
//                 {
//                     for (int i = 0; i < 4; i++)
//                     {
//                         xx = nx + dx[i];
//                         yy = ny + dy[i];
//                         if (stage[xx, yy] == 0)
//                         {
//                             stage[xx, yy] = 1;
//                             current_count++;
//                         }
//                     }
//                     nx = tx;
//                     ny = ty;
//                 }
//             }
//         }

//         for (int i = 0; i < stage.GetLength(0); i++)
//         {
//             for (int j = 0; j < stage.GetLength(1); j++)
//             {
//                 Debug.Log(stage[i, j]);
//             }
//         }
//     }
//     // Start is called before the first frame update
//     void Start()
//     {
//         stage = InitStage();
//         RandomStage();
//     }

//     // Update is called once per frame
//     void Update()
//     {

//     }
// }
