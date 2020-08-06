using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using MonoGame;

namespace SpaceShooter
{
    public interface QuadStorable
    {
        Rectangle Rect { get; }
        bool HasMoved { get; }
    }

    internal class QuadTreeObject<T> where T : QuadStorable
    {
        public T Data
        {
            get;
            private set;
        }
        internal QuadTreeNode<T> Owner 
        { 
            get; 
            set; 
        }
        public QuadTreeObject(T data)
        {
            Data = data;
        }
    }

    public class QuadTree<T> : ICollection<T> where T : QuadStorable
    {
        private readonly Dictionary<T, QuadTreeObject<T>> wrappedDictionary = new Dictionary<T, QuadTreeObject<T>>();
        private readonly QuadTreeNode<T> quadTreeRoot;

        public QuadTree(Rectangle rect)
        {
            quadTreeRoot = new QuadTreeNode<T>(rect);
        }

        public QuadTree(int x, int y, int width, int height)
        {
            quadTreeRoot = new QuadTreeNode<T>(new Rectangle(x, y, width, height));
        }

        public Rectangle QuadRect
        {
            get { return quadTreeRoot.QuadRect; }
        }

        public void DrawQuad(SpriteBatch spriteBatch, QuadTreeNode<T> quad, int depth)
        {
            if (quad != null)
            {
                Rectangle rect = quad.QuadRect;

                Color drawColor = Color.White;
                switch (depth)
                {
                    case 0:
                        drawColor = Color.White;
                        break;
                    case 1:
                        drawColor = Color.Red;
                        break;
                    case 2:
                        drawColor = Color.Green;
                        break;
                    case 3:
                        drawColor = Color.Blue;
                        break;
                    case 4:
                        drawColor = Color.Gray;
                        break;
                    case 5:
                        drawColor = Color.DarkRed;
                        break;
                    case 6:
                        drawColor = Color.DarkGreen;
                        break;
                    case 7:
                        drawColor = Color.DarkBlue;
                        break;
                    default:
                        drawColor = Color.White;
                        break;
                }

                //Primative.Box(rect.Left, rect.Top, rect.Right, rect.Bottom, 1, drawColor);
                Primitives2D.DrawRectangle(spriteBatch, rect, drawColor, 1);

                DrawQuad(spriteBatch, quad.TopLeftChild, depth + 1);
                DrawQuad(spriteBatch, quad.TopRightChild, depth + 1);
                DrawQuad(spriteBatch, quad.BottomLeftChild, depth + 1);
                DrawQuad(spriteBatch, quad.BottomRightChild, depth + 1);
            }
        }

        public List<T> GetObjects<J>(Rectangle rect)
        {
            return quadTreeRoot.GetObjects(rect).FindAll(delegate(T item)
            {
                return item is J;
            });
        }

        public void GetObjects(Rectangle rect, ref List<T> results)
        {
            quadTreeRoot.GetObjects(rect, ref results);
        }

        public List<T> GetAllObjects()
        {
            return new List<T>(wrappedDictionary.Keys);
        }

        public bool Move(T item)
        {
            if (Contains(item))
            {
                quadTreeRoot.Move(wrappedDictionary[item]);
                return true;
            }
            return false;
        }

        public void Add(T item)
        {
            QuadTreeObject<T> wrappedObject = new QuadTreeObject<T>(item);
            wrappedDictionary.Add(item, wrappedObject);
            quadTreeRoot.Insert(wrappedObject);
        }

        public void Clear()
        {
            wrappedDictionary.Clear();
            quadTreeRoot.Clear();
        }

        public bool Contains(T item)
        {
            return wrappedDictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            wrappedDictionary.Keys.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return wrappedDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (Contains(item))
            {
                quadTreeRoot.Delete(wrappedDictionary[item], true);
                wrappedDictionary.Remove(item);
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return wrappedDictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public QuadTreeNode<T> RootQuad
        {
            get { return quadTreeRoot; }
        }
    }

    public class QuadTreeNode<T> where T : QuadStorable
    {
        
        private const int maxObjectsPerNode = 2;

        private Rectangle rect;
        private List<QuadTreeObject<T>> objects = null;
        private QuadTreeNode<T> parent = null;

        private QuadTreeNode<T> childTL = null;
        private QuadTreeNode<T> childTR = null;
        private QuadTreeNode<T> childBL = null;
        private QuadTreeNode<T> childBR = null;

        public Rectangle QuadRect 
        {
            get { return rect; }
        }

        public QuadTreeNode<T> TopLeftChild
        {
            get { return childTL; }
        }

        public QuadTreeNode<T> TopRightChild
        {
            get { return childTR; }
        }

        public QuadTreeNode<T> BottomLeftChild
        {
            get { return childBL; }
        }

        public QuadTreeNode<T> BottomRightChild
        {
            get { return childBR; }
        }

        public QuadTreeNode<T> Parent
        {
            get { return parent; }
        }

        internal List<QuadTreeObject<T>> Objects
        {
            get { return objects; }
        }

        public int Count
        {
            get { return ObjectCount(); }
        }

        public bool IsEmptyLeaf
        {
            get { return Count == 0 && childTL == null; }
        }

        public QuadTreeNode(Rectangle rect)
        {
            this.rect = rect;
        }

        public QuadTreeNode(int x, int y, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
        }

        private QuadTreeNode(QuadTreeNode<T> parent, Rectangle rect)
            : this(rect)
        {
            this.parent = parent;
        }

        private void Add(QuadTreeObject<T> item)
        {
            if (objects == null)
            {
                objects = new List<QuadTreeObject<T>>();
            }

            item.Owner = this;
            objects.Add(item);
        }

        private void Remove(QuadTreeObject<T> item)
        {
            if (objects != null)
            {
                int removeIndex = objects.IndexOf(item);
                if (removeIndex >= 0)
                {
                    objects[removeIndex] = objects[objects.Count - 1];
                    objects.RemoveAt(objects.Count - 1);
                }
            }
        }

        private int ObjectCount()
        {
            int count = 0;

            // Add the objects at this level
            if (objects != null)
            {
                count += objects.Count;
            }

            // Add the objects that are contained in the children
            if (childTL != null)
            {
                count += childTL.ObjectCount();
                count += childTR.ObjectCount();
                count += childBL.ObjectCount();
                count += childBR.ObjectCount();
            }

            return count;
        }

        private void Subdivide()
        {
            Point size = new Point(rect.Width / 2, rect.Height / 2);
            Point mid = new Point(rect.X + size.X, rect.Y + size.Y);

            childTL = new QuadTreeNode<T>(this, new Rectangle(rect.Left, rect.Top, size.X, size.Y));
            childTR = new QuadTreeNode<T>(this, new Rectangle(mid.X, rect.Top, size.X, size.Y));
            childBL = new QuadTreeNode<T>(this, new Rectangle(rect.Left, mid.Y, size.X, size.Y));
            childBR = new QuadTreeNode<T>(this, new Rectangle(mid.X, mid.Y, size.X, size.Y));

            for (int i = 0; i < objects.Count; i++)
            {
                QuadTreeNode<T> destTree = GetDestinationTree(objects[i]);

                if (destTree != this)
                {
                    destTree.Insert(objects[i]);
                    Remove(objects[i]);
                    i--;
                }
            }
        }

        private QuadTreeNode<T> GetDestinationTree(QuadTreeObject<T> item)
        {
            // If a child can't contain an object, it will live in this Quad
            QuadTreeNode<T> destTree = this;

            if (childTL.QuadRect.Contains(item.Data.Rect))
            {
                destTree = childTL;
            }
            else if (childTR.QuadRect.Contains(item.Data.Rect))
            {
                destTree = childTR;
            }
            else if (childBL.QuadRect.Contains(item.Data.Rect))
            {
                destTree = childBL;
            }
            else if (childBR.QuadRect.Contains(item.Data.Rect))
            {
                destTree = childBR;
            }

            return destTree;
        }

        private void Relocate(QuadTreeObject<T> item)
        {
            // Are we still inside our parent?
            if (QuadRect.Contains(item.Data.Rect))
            {
                // Good, have we moved inside any of our children?
                if (childTL != null)
                {
                    QuadTreeNode<T> dest = GetDestinationTree(item);
                    if (item.Owner != dest)
                    {
                        // Delete the item from this quad and add it to our child
                        // Note: Do NOT clean during this call, it can potentially delete our destination quad
                        QuadTreeNode<T> formerOwner = item.Owner;
                        Delete(item, false);
                        dest.Insert(item);

                        // Clean up ourselves
                        formerOwner.CleanUpwards();
                    }
                }
            }
            else
            {
                // We don't fit here anymore, move up, if we can
                if (parent != null)
                {
                    parent.Relocate(item);
                }
            }
        }

        private void CleanUpwards()
        {
            if (childTL != null)
            {
                // If all the children are empty leaves, delete all the children
                if (childTL.IsEmptyLeaf &&
                    childTR.IsEmptyLeaf &&
                    childBL.IsEmptyLeaf &&
                    childBR.IsEmptyLeaf)
                {
                    childTL = null;
                    childTR = null;
                    childBL = null;
                    childBR = null;

                    if (parent != null && Count == 0)
                    {
                        parent.CleanUpwards();
                    }
                }
            }
            else
            {
                // I could be one of 4 empty leaves, tell my parent to clean up
                if (parent != null && Count == 0)
                {
                    parent.CleanUpwards();
                }
            }
        }

        internal void Insert(QuadTreeObject<T> item)
        {
            // If this quad doesn't contain the items rectangle, do nothing, unless we are the root
            if (!rect.Contains(item.Data.Rect))
            {
                System.Diagnostics.Debug.Assert(parent == null, "We are not the root, and this object doesn't fit here. How did we get here?");
                if (parent == null)
                {
                    // This object is outside of the QuadTree bounds, we should add it at the root level
                    Add(item);
                }
                else
                {
                    return;
                }
            }

            if (objects == null ||
                (childTL == null && objects.Count + 1 <= maxObjectsPerNode))
            {
                // If there's room to add the object, just add it
                Add(item);
            }
            else
            {
                // No quads, create them and bump objects down where appropriate
                if (childTL == null)
                {
                    Subdivide();
                }

                // Find out which tree this object should go in and add it there
                QuadTreeNode<T> destTree = GetDestinationTree(item);
                if (destTree == this)
                {
                    Add(item);
                }
                else
                {
                    destTree.Insert(item);
                }
            }
        }

        internal void Clear()
        {
            // Clear out the children, if we have any
            if (childTL != null)
            {
                childTL.Clear();
                childTR.Clear();
                childBL.Clear();
                childBR.Clear();
            }

            // Clear any objects at this level
            if (objects != null)
            {
                objects.Clear();
                objects = null;
            }

            // Set the children to null
            childTL = null;
            childTR = null;
            childBL = null;
            childBR = null;
        }

        internal void Delete(QuadTreeObject<T> item, bool clean)
        {
            if (item.Owner != null)
            {
                if (item.Owner == this)
                {
                    Remove(item);
                    if (clean)
                    {
                        CleanUpwards();
                    }
                }
                else
                {
                    item.Owner.Delete(item, clean);
                }
            }
        }

        internal List<T> GetObjects(Rectangle searchRect)
        {
            List<T> results = new List<T>();
            GetObjects(searchRect, ref results);
            return results;
        }

        internal void GetObjects(Rectangle searchRect, ref List<T> results)
        {
            // We can't do anything if the results list doesn't exist
            if (results != null)
            {
                if (searchRect.Contains(this.rect))
                {
                    // If the search area completely contains this quad, just get every object this quad and all it's children have
                    GetAllObjects(ref results);
                }
                else if (searchRect.Intersects(this.rect))
                {
                    // Otherwise, if the quad isn't fully contained, only add objects that intersect with the search rectangle
                    if (objects != null)
                    {
                        for (int i = 0; i < objects.Count; i++)
                        {
                            if (searchRect.Intersects(objects[i].Data.Rect))
                            {
                                results.Add(objects[i].Data);
                            }
                        }
                    }

                    // Get the objects for the search rectangle from the children
                    if (childTL != null)
                    {
                        childTL.GetObjects(searchRect, ref results);
                        childTR.GetObjects(searchRect, ref results);
                        childBL.GetObjects(searchRect, ref results);
                        childBR.GetObjects(searchRect, ref results);
                    }
                }
            }
        }

        internal void GetAllObjects(ref List<T> results)
        {
            // If this Quad has objects, add them
            if (objects != null)
            {
                foreach (QuadTreeObject<T> qto in objects)
                {
                    results.Add(qto.Data);
                }
            }

            // If we have children, get their objects too
            if (childTL != null)
            {
                childTL.GetAllObjects(ref results);
                childTR.GetAllObjects(ref results);
                childBL.GetAllObjects(ref results);
                childBR.GetAllObjects(ref results);
            }
        }

        internal void Move(QuadTreeObject<T> item)
        {
            if (item.Owner != null)
            {
                item.Owner.Relocate(item);
            }
            else
            {
                Relocate(item);
            }
        }

    }
}
