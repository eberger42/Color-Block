using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Assets.Editor.Components.Base
{
    internal abstract class QueueEditorComponentBase<T> : EditorComponentBase
    {
        protected List<T> _queue = new List<T>();
        protected ReorderableList _reorderableList;

        public QueueEditorComponentBase()
        {
        }

        public override void OnEnable()
        {
            if (_queue == null)
                _queue = new List<T>();

            _reorderableList = new ReorderableList(_queue, typeof(T), true, true, true, true);

            _reorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Block Queue");
            };

            _reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                UseDrawQueueItem(_queue[index], rect);

            };

            _reorderableList.onAddCallback = list =>
            {
                _queue.Add(default);
            };

            _reorderableList.onRemoveCallback = list =>
            {
                _queue.RemoveAt(list.index);
            };

        }

        public override void OnGUI()
        {
            if (_reorderableList == null)
                OnEnable();
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            _reorderableList.DoLayoutList();
            EditorGUILayout.EndVertical();

        }

        public List<T> GetQueueConfiguration()
        {
            return _queue.ToList();
        }

        protected abstract void UseDrawQueueItem(T item, Rect rect);
    }
}
