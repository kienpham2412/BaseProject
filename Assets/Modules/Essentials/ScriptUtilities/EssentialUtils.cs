using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Essential
{
    public static class Input
    {
        public static bool IsPressedInFrame
        {
            get
            {
#if UNITY_EDITOR
                return Mouse.current.leftButton.wasPressedThisFrame;
#else
                return Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
#endif
            }
        }

        public static bool IsPressing
        {
            get
            {
#if UNITY_EDITOR
                return Mouse.current.leftButton.isPressed;
#else
                return Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
#endif
            }
        }

        public static bool IsReleased
        {
            get
            {
#if UNITY_EDITOR
                return Mouse.current.leftButton.wasReleasedThisFrame;
#else
                return Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;
#endif
            }
        }

        public static Vector2 InputPosition
        {
            get
            {
#if UNITY_EDITOR

                return Mouse.current.position.ReadValue();
#else
                return Touchscreen.current.primaryTouch.position.ReadValue();
#endif
            }
        }

        public static Vector2 InputDelta
        {
            get
            {
#if UNITY_EDITOR

                return Mouse.current.delta.ReadValue();
#else
                return Touchscreen.current.primaryTouch.delta.ReadValue();
#endif
            }
        }
    }

    public static class Utils
    {
        #region Touch test

        private static List<RaycastResult> raycastResults;
        private static List<RaycastResult> RaycastResults => raycastResults ??= new List<RaycastResult>();

        /// <summary>
        /// Get all ui elements along the casted ray from pointer position <br/>
        /// This function will invoke an error if there is no canvas overlay or screen space camera in scene
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <returns></returns>
        internal static List<RaycastResult> GetEventSystemRaycastResults
            (this MonoBehaviour monoBehaviour)
        {
            RaycastResults.Clear();
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.InputPosition;
            EventSystem.current.RaycastAll(eventData, RaycastResults);
            return RaycastResults;
        }

        /// <summary>
        /// Check if current pointer is pointing over an ui element <br/>
        /// This function will invoke an error if there is no canvas overlay or screen space camera in scene
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <returns></returns>
        internal static bool IsPointingOverUIElement(this MonoBehaviour monoBehaviour)
        {
            return GetEventSystemRaycastResults(monoBehaviour).Count > 0;
        }

        internal static bool IsPointingOverUIElement(this MonoBehaviour monoBehaviour, LayerMask layerMask)
        {
            var elements = GetEventSystemRaycastResults(monoBehaviour);
            foreach (var e in elements)
            {
                if ((layerMask & (1 << e.gameObject.layer)) != 0) return true;
            }

            return false;
        }

        #endregion

        #region ArrayNList

        internal static void MoveFirstToLast<T>(this Queue<T> queue)
        {
            var first = queue.Dequeue();
            queue.Enqueue(first);
        }

        /// <summary>
        /// Return a random member of list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        internal static T Random<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Return a random member of array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static T Random<T>(this T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        internal static void Shuffle<T>(this List<T> list)
        {
            int index;
            T temp;
            for (int i = list.Count - 1; i >= 1; i--)
            {
                index = UnityEngine.Random.Range(0, i);
                temp = list[i];
                list[i] = list[index];
                list[index] = temp;
            }
        }

        internal static void Shuffle<T>(this T[] list)
        {
            int index;
            T temp;
            for (int i = list.Length - 1; i >= 1; i--)
            {
                index = UnityEngine.Random.Range(0, i);
                temp = list[i];
                list[i] = list[index];
                list[index] = temp;
            }
        }

        internal static void AddNoise<T>(this List<T> list, float intensity)
        {
            int swaps = (int)(list.Count * intensity);
            for (int k = 0; k < swaps; k++)
            {
                int a = UnityEngine.Random.Range(0, list.Count);
                int b = UnityEngine.Random.Range(0, list.Count);
                (list[a], list[b]) = (list[b], list[a]);
            }
        }

        internal static int Total(this int[] array)
        {
            var result = 0;
            foreach (var i in array) result += i;
            return result;
        }

        internal static void DefaultValues<T>(this T[] array)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = default;
        }

        public static void TryAdd<T>(this List<T> list, T element)
        {
            if (!list.Contains(element))
            {
                list.Add(element);
            }
        }

        public static void ToArrayBuffer<T>(this List<T> list, T[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (i >= list.Count) return;
                buffer[i] = list[i];
            }
        }

        public static void AddRange<T>(this HashSet<T> hashSet, T[] array)
        {
            foreach (var a in array)
                hashSet.Add(a);
        }

        public static T GetAtIndex<T>(this T[] array, int index)
        {
            index = Mathf.Clamp(index, 0, array.Length - 1);
            return array[index];
        }

        #endregion

        #region Debug draw

#if UNITY_EDITOR
        public static void DrawLine(Vector3 from, Vector3 to, Color color, float thickness, float offsetHead,
            float offsetTail)
        {
            var dir = (to - from).normalized;
            CalculateOffset(from, to, offsetHead, offsetTail, out Vector3 outFrom, out Vector3 outTo);
            DrawLine(outFrom, outTo, color, thickness);
        }

        public static void DrawLine(Vector3 from, Vector3 to, Color color, float thickness)
        {
            Handles.color = color;
            Handles.DrawLine(from, to, thickness);
        }

        public static void DrawArrow(Vector3 from, Vector3 to, Color color, float thickness, float offsetHead,
            float offsetTail, float headLength)
        {
            var dir = CalculateOffset(from, to, offsetHead, offsetTail, out Vector3 outFrom, out Vector3 outTo);
            DrawLine(outFrom, outTo, color, thickness);
            DrawArrowHead(dir, outTo, color, thickness, headLength);
        }

        public static void DrawArrow(Vector3 from, Vector3 to, Color color)
        {
            DrawArrow(from, to, color, 4, 0.3f, 0.3f, 0.1f);
        }

        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            DrawLine(from, to, color, 4, 0.3f, 0.3f);
        }

        private static void DrawArrowHead(Vector3 dir, Vector3 head, Color color, float thickness, float length)
        {
            var headDir = dir.normalized * length;
            DrawLine(head, head + Quaternion.Euler(0, 30, 0) * -headDir, color, thickness);
            DrawLine(head, head + Quaternion.Euler(0, -30, 0) * -headDir, color, thickness);
        }

        private static Vector3 CalculateOffset(Vector3 from, Vector3 to, float offsetHead, float offsetTail,
            out Vector3 outFrom, out Vector3 outTo)
        {
            var dir = (to - from).normalized;
            outFrom = from + dir * offsetHead;
            outTo = to - dir * offsetTail;
            return dir;
        }

        public static void DrawSphere(Vector3 position, Color color, float radius)
        {
            Handles.color = color;
            Handles.SphereHandleCap(0, position, Quaternion.identity, radius * 2, EventType.Repaint);
        }

        public static void DrawPath(List<Vector3> points)
        {
            for (int i = 1; i < points.Count; i++)
                Debug.DrawLine(points[i - 1], points[i], Color.red, 5f);
        }

        public static void DrawSquare(Vector3 center, Color color, float size, Vector3 eulerAngle)
        {
            float halfSize = size / 2f;

            // Các điểm góc trong local space (trên mặt phẳng XZ)
            Vector3[] corners = new Vector3[4];
            corners[0] = new Vector3(-halfSize, halfSize, 0); // top-left
            corners[1] = new Vector3(halfSize, halfSize, 0); // top-right
            corners[2] = new Vector3(halfSize, -halfSize, 0); // bottom-right
            corners[3] = new Vector3(-halfSize, -halfSize, 0); // bottom-left

            // Áp dụng rotation
            Quaternion rotation = Quaternion.Euler(eulerAngle);
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = center + rotation * corners[i];
            }

            // Vẽ các cạnh
            for (int i = 0; i < corners.Length; i++)
            {
                DrawLine(corners[i], corners[(i + 1) % corners.Length], color, 1f);
            }
        }

        public static void DrawRectangle(Vector3 center, Color color, Vector2 size, Vector3 eulerAngle)
        {
            var halfSize = size * 0.5f;

            // Các điểm góc trong local space (trên mặt phẳng XZ)
            Vector3[] corners = new Vector3[4];
            corners[0] = new Vector3(-halfSize.x, halfSize.y, 0); // top-left
            corners[1] = new Vector3(halfSize.x, halfSize.y, 0); // top-right
            corners[2] = new Vector3(halfSize.x, -halfSize.y, 0); // bottom-right
            corners[3] = new Vector3(-halfSize.x, -halfSize.y, 0); // bottom-left

            // Áp dụng rotation
            Quaternion rotation = Quaternion.Euler(eulerAngle);
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = center + rotation * corners[i];
            }

            // Vẽ các cạnh
            for (int i = 0; i < corners.Length; i++)
            {
                DrawLine(corners[i], corners[(i + 1) % corners.Length], color, 1f);
            }
        }
#endif

        #endregion

        #region Miscellaneous

        public static bool IsLastChild(this Transform transform)
        {
            var parent = transform.parent;
            return transform.GetSiblingIndex() == parent.childCount - 1;
        }

        public static bool IsFirstChild(this Transform transform)
        {
            var parent = transform.parent;
            return transform.GetSiblingIndex() == 0;
        }

        /// <summary>
        /// Take the nearest transform calculated from the reference point
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="referencePoint"></param>
        /// <returns></returns>
        internal static Transform GetNearest(this RaycastHit[] hits, Vector3 referencePoint)
        {
            var result = hits[0].transform;
            Vector3 dirToRef = referencePoint - hits[0].point;
            var nearestDistance = Vector3.SqrMagnitude(dirToRef);

            for (int i = 0; i < hits.Length - 1; i++)
            {
                if (hits[i].transform == null) continue;
                dirToRef = referencePoint - hits[i].point;
                var distance = Vector3.SqrMagnitude(dirToRef);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    result = hits[i].transform;
                }
            }

            return result;
        }

        /// <summary>
        /// Calculate the modulo of a value in a circular manner.
        /// </summary>
        /// <param name="value">The value to calculate the modulo of.</param>
        /// <param name="step">The step value to add to the original value.</param>
        /// <param name="length">The length of the circle.</param>
        /// <returns>The result of the circular modulo calculation.</returns>
        internal static int CircleModulo(this int value, int step, int length)
        {
            return (value + step) % length;
        }

        /// <summary>
        /// Calculate the reverse modulo of a value in a circular manner.
        /// </summary>
        /// <param name="value">The value to calculate the reverse modulo of.</param>
        /// <param name="step">The step value to subtract from the original value.</param>
        /// <param name="length">The length of the circle.</param>
        /// <returns>The result of the reverse circular modulo calculation.</returns>
        internal static int ReverseCircleModulo(this int value, int step, int length)
        {
            var subtract = value - step;
            return subtract >= 0 ? subtract : length + subtract;
        }

        internal static List<T> GetComponentsInDirectChildren<T>(this Transform parent, bool includeInactive = false)
            where T : Component
        {
            List<T> list = new List<T>(parent.childCount);

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (!includeInactive && !child.gameObject.activeInHierarchy) continue;
                var c = child.GetComponent<T>();
                if (c != null) list.Add(c);
            }

            return list;
        }

        internal static void GetComponentsInDirectChildren<T>(this Transform parent, ref List<T> buffer,
            bool includeInactive = false)
            where T : Component
        {
            buffer.Clear();

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (!includeInactive && !child.gameObject.activeInHierarchy) continue;
                var c = child.GetComponent<T>();
                if (c != null) buffer.Add(c);
            }
        }

        internal static void DestroyChildren(this Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i);
#if UNITY_EDITOR
                if (Application.isPlaying)
                    GameObject.Destroy(child.gameObject);
                else GameObject.DestroyImmediate(child.gameObject);
#else
                GameObject.Destroy(child.gameObject);
#endif
            }
        }

        public static int EncodeWeightedSum(this string input)
        {
            int sum = 0;
            for (int i = 0; i < input.Length; i++)
            {
                sum += (i + 1) * input[i];
            }

            return sum;
        }

#if UNITY_EDITOR
        public static int CountLevelPrefabs()
        {
            string targetFolder = "Assets/Modules/FroggyBouncy/Prefabs/Level/Levels";

            if (!Directory.Exists(targetFolder))
            {
                Debug.LogError($"Folder không tồn tại: {targetFolder}");
                return 0;
            }

            // Lấy tất cả các file *.prefab trong folder
            string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { targetFolder });

            return prefabGUIDs.Length;
        }
#endif

        #endregion

        #region Canvas converter

        /// <summary>
        /// Convert a screen point to a canvas point.
        /// </summary>
        /// <param name="screenPoint">The screen point to convert.</param>
        /// <param name="currentCanvas">The canvas to convert the point to.</param>
        /// <returns>The converted canvas point.</returns>
        public static Vector2 ConvertScreenToCanvasPoint(this Vector2 screenPoint, Canvas currentCanvas)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                currentCanvas.GetComponent<RectTransform>(),
                screenPoint,
                currentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : currentCanvas.worldCamera,
                out Vector2 posCavasOverlay
            );
            return posCavasOverlay;
        }

        /// <summary>
        /// Convert a world point to a screen point.
        /// </summary>
        /// <param name="point">The world point to convert.</param>
        /// <param name="currentCanvas">The canvas to use for the conversion (optional).</param>
        /// <returns>The converted screen point.</returns>
        public static Vector2 ConvertToScreenPoint(this Vector3 point, Canvas currentCanvas = null)
        {
            if (currentCanvas == null)
                return RectTransformUtility.WorldToScreenPoint(Camera.main, point);
            else
                return RectTransformUtility.WorldToScreenPoint(
                    currentCanvas.renderMode == RenderMode.ScreenSpaceCamera ? currentCanvas.worldCamera : null, point);
        }

        /// <summary>
        /// Convert a world space point to a canvas point.
        /// </summary>
        /// <param name="point">The world space point to convert.</param>
        /// <param name="currentCanvas">The current canvas to use for the conversion.</param>
        /// <param name="canvasTaget">The target canvas to convert the point to.</param>
        /// <returns>The converted canvas point.</returns>
        public static Vector2 ConvertWorldSpaceToCanvasPoint(this Vector3 point, Canvas currentCanvas,
            Canvas canvasTaget)
        {
            return ConvertToScreenPoint(point, currentCanvas).ConvertScreenToCanvasPoint(canvasTaget);
        }

        #endregion

        #region Tween

        public static Tween DoPlayAnimation(this Animator animator, int stateHash, float duration)
        {
            return DOTween.To(() => 0, UpdateAnimation, 1f, duration);
            
            void UpdateAnimation(float t)
            {
                animator.Play(stateHash, -1, t);
            }
        }
        
        public static Tweener DoInt(this TMP_Text text, int startValue, int endValue, float duration, string format)
        {
            return DOTween.To(() => startValue, t => { text.SetText(format, t); }, endValue, duration);
        }

        #endregion
        
        #region Time
        public static double ConvertToUnixTime(this DateTime time)
        {
            DateTime epoch = new System.DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return (time - epoch).TotalSeconds;
        }

        public static DateTime ConvertFromUnixTime(this double timeStamp)
        {
            DateTime epoch = new System.DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime time = epoch.AddSeconds(timeStamp);
            return time;
        }
        
        public static void ConvertSecondsToMMSS(this int totalSeconds, StringBuilder timeFormat)
        {
            // Tính số phút và giây
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            // Tạo chuỗi mm:ss bằng StringBuilder
            timeFormat ??= new StringBuilder(5);
            timeFormat.Clear();
            timeFormat.Append(minutes.ToString("00")); // Đảm bảo luôn có 2 chữ số
            timeFormat.Append(":");
            timeFormat.Append(seconds.ToString("00")); // Đảm bảo luôn có 2 chữ số
        }

        #endregion
    }

    #region Extend Debug

    public static class ExtDebug
    {
        /// <summary>
        /// Draws just the box at where it is currently hitting.
        /// </summary>
        /// <param name="origin">The origin point of the box cast.</param>
        /// <param name="halfExtents">The half extents of the box cast.</param>
        /// <param name="orientation">The orientation of the box cast.</param>
        /// <param name="direction">The direction of the box cast.</param>
        /// <param name="hitInfoDistance">The distance to the hit point.</param>
        /// <param name="color">The color to draw the box cast.</param>
        /// <param name="duration">The duration to draw the box cast.</param>
        public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation,
            Vector3 direction, float hitInfoDistance, Color color, float duration)
        {
            origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
            DrawBox(origin, halfExtents, orientation, color, duration);
        }

        /// <summary>
        /// Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance.
        /// </summary>
        /// <param name="origin">The origin point of the box cast.</param>
        /// <param name="halfExtents">The half extents of the box cast.</param>
        /// <param name="orientation">The orientation of the box cast.</param>
        /// <param name="direction">The direction of the box cast.</param>
        /// <param name="distance">The distance of the box cast.</param>
        /// <param name="color">The color to draw the box cast.</param>
        /// <param name="duration">The duration to draw the box cast.</param>
        public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation,
            Vector3 direction, float distance, Color color, float duration)
        {
            direction.Normalize();
            BoxCast bottomBox = new BoxCast(origin, halfExtents, orientation);
            BoxCast topBox = new BoxCast(origin + (direction * distance), halfExtents, orientation);

            // Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft,	color);
            // Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
            // Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
            // Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight,	color);
            // Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft,	color);
            // Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
            // Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
            // Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight,	color);

            DrawBox(bottomBox, color, duration);
            DrawBox(topBox, color, duration);
        }

        /// <summary>
        /// Draw a box at the specified origin with the given half extents and orientation.
        /// </summary>
        /// <param name="origin">The origin point of the box.</param>
        /// <param name="halfExtents">The half extents of the box.</param>
        /// <param name="orientation">The orientation of the box.</param>
        /// <param name="color">The color to draw the box.</param>
        /// <param name="duration">The duration to draw the box.</param>
        public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color,
            float duration)
        {
            DrawBox(new BoxCast(origin, halfExtents, orientation), color, duration);
        }

        /// <summary>
        /// Draw a box using the specified BoxCast structure.
        /// </summary>
        /// <param name="box">The BoxCast structure to use for drawing the box.</param>
        /// <param name="color">The color to draw the box.</param>
        /// <param name="duration">The duration to draw the box.</param>
        public static void DrawBox(BoxCast box, Color color, float duration)
        {
            Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color, duration);
            Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color, duration);
            Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color, duration);
            Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color, duration);

            Debug.DrawLine(box.backTopLeft, box.backTopRight, color, duration);
            Debug.DrawLine(box.backTopRight, box.backBottomRight, color, duration);
            Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color, duration);
            Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color, duration);

            Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color, duration);
            Debug.DrawLine(box.frontTopRight, box.backTopRight, color, duration);
            Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color, duration);
            Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color, duration);
        }

        public struct BoxCast
        {
            public Vector3 localFrontTopLeft { get; private set; }
            public Vector3 localFrontTopRight { get; private set; }
            public Vector3 localFrontBottomLeft { get; private set; }
            public Vector3 localFrontBottomRight { get; private set; }

            public Vector3 localBackTopLeft
            {
                get { return -localFrontBottomRight; }
            }

            public Vector3 localBackTopRight
            {
                get { return -localFrontBottomLeft; }
            }

            public Vector3 localBackBottomLeft
            {
                get { return -localFrontTopRight; }
            }

            public Vector3 localBackBottomRight
            {
                get { return -localFrontTopLeft; }
            }

            public Vector3 frontTopLeft
            {
                get { return localFrontTopLeft + origin; }
            }

            public Vector3 frontTopRight
            {
                get { return localFrontTopRight + origin; }
            }

            public Vector3 frontBottomLeft
            {
                get { return localFrontBottomLeft + origin; }
            }

            public Vector3 frontBottomRight
            {
                get { return localFrontBottomRight + origin; }
            }

            public Vector3 backTopLeft
            {
                get { return localBackTopLeft + origin; }
            }

            public Vector3 backTopRight
            {
                get { return localBackTopRight + origin; }
            }

            public Vector3 backBottomLeft
            {
                get { return localBackBottomLeft + origin; }
            }

            public Vector3 backBottomRight
            {
                get { return localBackBottomRight + origin; }
            }

            public Vector3 origin { get; private set; }

            public BoxCast(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
            {
                Rotate(orientation);
            }

            public BoxCast(Vector3 origin, Vector3 halfExtents)
            {
                this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
                this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

                this.origin = origin;
            }


            public void Rotate(Quaternion orientation)
            {
                localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
                localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
                localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
                localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
            }
        }

        //This should work for all cast types
        static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
        {
            return origin + (direction.normalized * hitInfoDistance);
        }

        static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            Vector3 direction = point - pivot;
            return pivot + rotation * direction;
        }
    }

    #endregion
}