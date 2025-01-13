/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using System.Collections.Generic;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Utils;
using TouchScript.Pointers;
using TouchScript.Utils.Attributes;
using UnityEngine;
using UnityEngine.Profiling;

namespace TouchScript.Behaviors.Cursors
{
    /// <summary>
    /// <para>Pointer visualizer which shows pointer circles with debug text using Unity UI.</para>
    /// <para>The script should be placed on an element with RectTransform or a Canvas. A reference prefab is provided in TouchScript package.</para>
    /// </summary>
    [HelpURL("http://touchscript.github.io/docs/html/T_TouchScript_Behaviors_Cursors_CursorManager.htm")]
    public class ObjectOneCamera : MonoBehaviour
    {
        #region Public properties

        
        /// <summary>
        /// Prefab to use as object cursors template.
        /// </summary>
        public PointerCursor ObjectCursor
        {
            get { return objectCursor; }
            set { objectCursor = value; }
        }

        /// <summary>
        /// Gets or sets whether <see cref="CursorManager"/> is using DPI to scale pointer cursors.
        /// </summary>
        /// <value> <c>true</c> if DPI value is used; otherwise, <c>false</c>. </value>
        public bool UseDPI
        {
            get { return useDPI; }
            set
            {
                useDPI = value;
                updateCursorSize();
            }
        }

        /// <summary>
        /// Gets or sets the size of pointer cursors in cm. This value is only used when <see cref="UseDPI"/> is set to <c>true</c>.
        /// </summary>
        /// <value> The size of pointer cursors in cm. </value>
        public float CursorSize
        {
            get { return cursorSize; }
            set
            {
                cursorSize = value;
                updateCursorSize();
            }
        }

        /// <summary>
        /// Cursor size in pixels.
        /// </summary>
        public uint CursorPixelSize
        {
            get { return cursorPixelSize; }
            set
            {
                cursorPixelSize = value;
                updateCursorSize();
            }
        }

        public float PanSpeed = 200f;
        public float ZoomSpeed = 10f; 
        public Transform pivot;
        public Transform cam;
        public Transform CamOffset; 
        private float previousAngle;
        public float minFov = 10f;
        public float maxFov = 120f; 

        #endregion

        #region Private variables

        [SerializeField]
        private bool cursorsProps; // Used in the custom inspector

        [SerializeField]
        private PointerCursor objectCursor;

        [SerializeField] 
        private float degree;

        [SerializeField]
        [ToggleLeft]
        private bool useDPI = true;

        [SerializeField]
        private float cursorSize = 1f;

        [SerializeField]
        private uint cursorPixelSize = 64;

        private RectTransform rect;
        private ObjectPool<PointerCursor> objectPool;
        private Dictionary<int, PointerCursor> cursors = new Dictionary<int, PointerCursor>(10);

		private CustomSampler cursorSampler;
        
        private Vector3 pos;
        private float _placedAngle;

        
        #endregion

        #region Unity methods

        private void Awake()
        {

            pivot = Manager.GameManager.Pivot.transform;
            cam = Manager.GameManager.MRTK_XR_RIG_Camera.transform;
            CamOffset = Manager.GameManager.MRTK_XR_RIG_Camera_Offset.transform; 
            
            cursorSampler = CustomSampler.Create("[TouchScript] Update Cursors");

			cursorSampler.Begin();

            objectPool = new ObjectPool<PointerCursor>(5, instantiateObjectProxy, null, clearProxy);

            updateCursorSize();

            rect = transform as RectTransform;
            if (rect == null)
            {
                Debug.LogError("CursorManager must be on an UI element!");
                enabled = false;
            }

			cursorSampler.End();
        }

        private void OnEnable()
        {
            if (TouchManager.Instance != null)
            {
                TouchManager.Instance.PointersAdded += pointersAddedHandler;
                TouchManager.Instance.PointersRemoved += pointersRemovedHandler;
                TouchManager.Instance.PointersPressed += pointersPressedHandler;
                TouchManager.Instance.PointersReleased += pointersReleasedHandler;
                TouchManager.Instance.PointersUpdated += PointersUpdatedHandler;
                TouchManager.Instance.PointersCancelled += pointersCancelledHandler;
            }
        }

        private void OnDisable()
        {
            if (TouchManager.Instance != null)
            {
                TouchManager.Instance.PointersAdded -= pointersAddedHandler;
                TouchManager.Instance.PointersRemoved -= pointersRemovedHandler;
                TouchManager.Instance.PointersPressed -= pointersPressedHandler;
                TouchManager.Instance.PointersReleased -= pointersReleasedHandler;
                TouchManager.Instance.PointersUpdated -= PointersUpdatedHandler;
                TouchManager.Instance.PointersCancelled -= pointersCancelledHandler;
            }
        }


        #endregion

        #region Private functions

       private PointerCursor instantiateObjectProxy()
        {
            return Instantiate(objectCursor);
        }

        private void clearProxy(PointerCursor cursor)
        {
            cursor.Hide();
        }

        private void updateCursorSize()
        {
            if (useDPI) cursorPixelSize = (uint) (cursorSize * TouchManager.Instance.DotsPerCentimeter);
        }

        #endregion

        #region Event handlers

        private void pointersAddedHandler(object sender, PointerEventArgs e)
        {
			cursorSampler.Begin();

            updateCursorSize();

            var count = e.Pointers.Count;
            for (var i = 0; i < count; i++)
            {
                var pointer = e.Pointers[i];
                // Don't show internal pointers
                if ((pointer.Flags & Pointer.FLAG_INTERNAL) > 0) continue;

                PointerCursor cursor;
                if (pointer.Type == Pointer.PointerType.Object && pointer.ObjectId == 1)
                {
                            cursor = objectPool.Get();
                            cursor.Size = cursorPixelSize;
                            cursor.Init(rect, pointer);
                            cursors.Add(pointer.Id, cursor);
                            if(Manager.GameManager.RoomConnected)
                            {
                                if (Manager.GameManager.camControllers_Enabled)
                                {
                                    Manager.GameManager.CamControllerToggle();
                                    Debug.Log("Master Puck Placed with Angle saved " + _placedAngle + "     " + " Pointer Angle " + pointer.Angle);
                                }
                               
                                _placedAngle = pointer.Angle;
                                
                             
                                

                            } 
                }

               
            }

			cursorSampler.End();
        }

        private void pointersRemovedHandler(object sender, PointerEventArgs e)
        {
			cursorSampler.Begin();

            var count = e.Pointers.Count;
            for (var i = 0; i < count; i++)
            {
                var pointer = e.Pointers[i];
                PointerCursor cursor;
                if (!cursors.TryGetValue(pointer.Id, out cursor)) continue;
                cursors.Remove(pointer.Id);

                if (pointer.Type == Pointer.PointerType.Object && pointer.ObjectId == 1)
                {
                            objectPool.Release(cursor);
                           
                            if (!Manager.GameManager.camControllers_Enabled)
                            {
                                Debug.Log("Removed Master Puck");
                                Manager.GameManager.CamControllerToggle();
                            }
                          

                            
                }
                        
            }

			cursorSampler.End();
        }

        private void pointersPressedHandler(object sender, PointerEventArgs e)
        {
			cursorSampler.Begin();

            var count = e.Pointers.Count;
            for (var i = 0; i < count; i++)
            {
                var pointer = e.Pointers[i];
                PointerCursor cursor;
                if (!cursors.TryGetValue(pointer.Id, out cursor)) continue;
                if (pointer.Type == Pointer.PointerType.Object && pointer.ObjectId == 1)
                {
                            cursor.SetState(pointer, PointerCursor.CursorState.Pressed);
                            
                           
                }
                       
                
               
            }

			cursorSampler.End();
        }

        private void PointersUpdatedHandler(object sender, PointerEventArgs e)
        {
            
            
			cursorSampler.Begin();

            var count = e.Pointers.Count;
            for (var i = 0; i < count; i++)
            {
                var pointer = e.Pointers[i];
                PointerCursor cursor;
                if (!cursors.TryGetValue(pointer.Id, out cursor)) continue;
               
                if (pointer.Type == Pointer.PointerType.Object && pointer.ObjectId == 1)
                {
                    
                            float deltaAngle = Mathf.DeltaAngle(previousAngle , pointer.Angle);
                            Debug.Log("DEEEEEEEEEEEEEELTA " + deltaAngle*Mathf.Rad2Deg);
                            cursor.UpdatePointer(pointer);
                            degree = pointer.Angle * Mathf.Rad2Deg;
                            cursor.transform.rotation = Quaternion.Euler(0f, 0f, -degree);
                            pos = new Vector3(pointer.Position.x, 0f, pointer.Position.y);
                            Vector3 prev= new Vector3(pointer.PreviousPosition.x, 0f, pointer.PreviousPosition.y);
                            
                            pivot.localPosition += (prev-pos)*PanSpeed;
                            var fov = pivot.localPosition.y; 
                            if (deltaAngle < 0)
                            {        
                                fov += (degree / 360f)*-1f;
                                
                            }
                            else if(deltaAngle > 0)
                            {
                                fov += (degree / 360f);
                            }
                    
                            Debug.Log(fov);
                            var fov2 = Mathf.Clamp(fov, minFov, maxFov);
                            pivot.position = new Vector3(pivot.position.x, fov2, pivot.position.z); 

                            previousAngle = pointer.Angle;

                }
                        
                
            }

			cursorSampler.End();
        }

        private void pointersReleasedHandler(object sender, PointerEventArgs e)
        {
			cursorSampler.Begin();

            var count = e.Pointers.Count;
            for (var i = 0; i < count; i++)
            {
                var pointer = e.Pointers[i];
                PointerCursor cursor;
                if (!cursors.TryGetValue(pointer.Id, out cursor)) continue;
                
                if (pointer.Type == Pointer.PointerType.Object && pointer.ObjectId == 1)
                {
                            cursor.SetState(pointer, PointerCursor.CursorState.Released);
                           

                            
                }
                        
            }

			cursorSampler.End();
        }

        private void pointersCancelledHandler(object sender, PointerEventArgs e)
        {
            var count = e.Pointers.Count;
            for (var i = 0; i < count; i++)
            {
                var pointer = e.Pointers[i];
                PointerCursor cursor;
                if (!cursors.TryGetValue(pointer.Id, out cursor)) continue;
                
                if (pointer.Type == Pointer.PointerType.Object && pointer.ObjectId == 1)
                {
                   
                            pointersRemovedHandler(sender, e);
                           
                }
                       
            }
        }

        #endregion
    }
}