using System;
using System.Collections.Generic;

namespace Kiwi.Common
{
    public abstract class TransactableStreamBase<TItem> where TItem : class
    {
        private readonly List<TItem> _items;
        private int _index;
        private readonly Stack<int> _snapshotIndexes;

        protected TransactableStreamBase(Func<List<TItem>> getStreamItems)
        {
            _items = getStreamItems();
            _index = 0;
            _snapshotIndexes = new Stack<int>();
        }

        public TItem Current
        {
            get
            {
                if (End(0))
                {
                    return null;
                }

                return _items[_index];
            }
        }

        public void Consume()
        {
            _index++;
        }

        public TItem Peek(int offset)
        {
            if (End(offset))
            {
                return null;
            }

            return _items[_index + offset];
        }

        public void TakeSnapshot()
        {
            _snapshotIndexes.Push(_index);
        }

        public void RollbackSnapshot()
        {
            _index = _snapshotIndexes.Pop();
        }

        public void CommitSnapshot()
        {
            _snapshotIndexes.Pop();
        }

        private bool End(int offset)
        {
            return _items.Count <= _index + offset;
        }
    }
}