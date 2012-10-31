using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using PR2_SMS;

namespace SortManager
{
    #region Comparers
    [ComVisible(false)]
    public class ListViewTextSort: IComparer<ListViewItem>
    {
        public ListViewTextSort(Int32 sortColumn, Boolean ascending)
        {
            m_column = sortColumn;
            m_ascending = ascending;
        }
        protected virtual Int32 OnCompare(String lhs, String rhs)
        {
            return String.Compare(lhs, rhs, false);
        }
        private Int32 m_column;
        private Boolean m_ascending;
        #region IComparer<ListViewItem> Members

        public virtual int Compare(ListViewItem lhsLvi, ListViewItem rhsLvi)
        {
            //ListViewItem lhsLvi = lhs as ListViewItem;
            //ListViewItem rhsLvi = rhs as ListViewItem;
            if (lhsLvi == null || rhsLvi == null)    // We only know how to sort ListViewItems, so return equal
                return 0;

            ListViewItem.ListViewSubItemCollection lhsItems = lhsLvi.SubItems;
            ListViewItem.ListViewSubItemCollection rhsItems = rhsLvi.SubItems;

            String lhsText = (lhsItems.Count > m_column) ? lhsItems[m_column].Text : String.Empty;
            String rhsText = (rhsItems.Count > m_column) ? rhsItems[m_column].Text : String.Empty;

            Int32 result = 0;
            if (lhsText.Length == 0 || rhsText.Length == 0)
                result = lhsText.CompareTo(rhsText);

            else
                result = OnCompare(lhsText, rhsText);

            if (!m_ascending)
                result = -result;

            return result;
        }

        #endregion
    }
    [ComVisible(false)]
    public class ListViewTextCaseInsensitiveSort: ListViewTextSort
    {        
        public ListViewTextCaseInsensitiveSort(Int32 sortColumn, Boolean ascending):
            base(sortColumn, ascending)
        {
        }
        protected override Int32 OnCompare(String lhs, String rhs)
        {
            return String.Compare(lhs, rhs, true);
        }
    }
    [ComVisible(false)]
    public class ListViewDateSort: ListViewTextSort
    {
        public ListViewDateSort(Int32 sortColumn, Boolean ascending):
            base(sortColumn, ascending)
        {
        }
        protected override Int32 OnCompare(String lhs, String rhs)
        {
            return DateTime.Parse(lhs).CompareTo(DateTime.Parse(rhs));
        }
    }
    [ComVisible(false)]
    public class ListViewInt32Sort: ListViewTextSort
    {
        public ListViewInt32Sort(Int32 sortColumn, Boolean ascending):
            base(sortColumn, ascending)
        {
        }
        protected override Int32 OnCompare(String lhs, String rhs)
        {
            try
            {
                int res = Int32.Parse(lhs, NumberStyles.Number) - Int32.Parse(rhs, NumberStyles.Number);
                return res;
            }
            catch(FormatException)
            {
                return -1;
            }

        }
    }
    [ComVisible(false)]
	public class ListViewInt64Sort: ListViewTextSort
	{
		public ListViewInt64Sort(Int32 sortColumn, Boolean ascending):
			base(sortColumn, ascending)
		{
		}
		protected override Int32 OnCompare(String lhs, String rhs)
		{
			return (Int32)(Int64.Parse(lhs, NumberStyles.Number) - Int64.Parse(rhs, NumberStyles.Number));
		}
	}
    [ComVisible(false)]
    public class ListViewDoubleSort: ListViewTextSort
    {
        public ListViewDoubleSort(Int32 sortColumn, Boolean ascending):
            base(sortColumn, ascending)
        {
        }
        protected override Int32 OnCompare(String lhs, String rhs)
        {
            Double result = Double.Parse(lhs) - Double.Parse(rhs);

            if(result > 0)
                return 1;

            else if(result < 0)
                return -1;

            else
                return 0;
        }
    }    
    #endregion

    #region ListViewSortManager
    [ComVisible(false)]
    public class ListViewSorter
    {
        //internal event EditActions RefreshNow;
        private static bool s_useNativeArrows = true;//ComCtlDllSupportsArrows();
		public ListViewSorter(EnhancedListView list, Type[] comparers, Int32 column, SortOrder order)
		{
			m_column = -1;
			m_sortOrder = SortOrder.None;

			m_list = list;
			m_comparers = comparers;

            if (!s_useNativeArrows)
            {
                m_imgList = new ImageList();
                m_imgList.ImageSize = new Size(8, 8);
                m_imgList.TransparentColor = System.Drawing.Color.Magenta;

                m_imgList.Images.Add(GetArrowBitmap(ArrowType.Ascending));		// Add ascending arrow
                m_imgList.Images.Add(GetArrowBitmap(ArrowType.Descending));		// Add descending arrow

                m_list.SmallImageList = m_imgList;
                //SetHeaderImageList(m_list, m_imgList);
            }
            list.ColumnClick += new ColumnClickEventHandler(ColumnClick);
            if (column != -1)
                Sort(column, order);
		}
        internal ListViewSorter(EnhancedListView list, Type[] comparers):
			this(list, comparers, -1, SortOrder.None)
        {
		}
        private List<ListViewItem> additionalStorage;
        private bool filterEnabled;
		public Int32 Column
		{
			get { return m_column; }
		}
		public SortOrder SortOrder
		{
			get { return m_sortOrder; }
		}
		public Type GetColumnComparerType(Int32 column)
		{
			return m_comparers[column];
		}
		public void SetColumnComparerType(Int32 column, Type comparerType)
		{
			m_comparers[column] = comparerType;
		}
		public void SetComparerTypes(Type[] comparers)
		{
			m_comparers = comparers;
		}
        public void Sort(Int32 column)
        {
			SortOrder order = SortOrder.Ascending;
			
			if(column == m_column)
				order = (m_sortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;

			Sort(column, order);
        }
		public void Sort(Int32 column, SortOrder order)
		{
            try
            {
                if (column < 0 || column >= m_comparers.Length)
                    throw new IndexOutOfRangeException();

                if (column != m_column)
                {
                    ShowHeaderIcon(m_list, m_column, SortOrder.None);
                    m_column = column;
                }

                ShowHeaderIcon(m_list, m_column, order);
                m_sortOrder = order;

                if (m_sortOrder != SortOrder.None)
                {
                    ListViewTextSort comp = (ListViewTextSort)Activator.CreateInstance(m_comparers[m_column], new Object[] { m_column, m_sortOrder == SortOrder.Ascending });
                    if (!filterEnabled)
                        SC.LVCollection.Sort(comp);
                    else
                        additionalStorage.Sort(comp);
                }
                else
                    //m_list.ListViewItemSorter = null;
                    return;
                m_list.Refresh();
            }
            catch (Exception)
            {
               //&&&
            }
		}
		public Boolean SortEnabled
		{
			get 
			{ 
				return m_list.ListViewItemSorter != null; 
			}

			set
			{
				if(value)
				{
					if(!this.SortEnabled)
					{
						m_list.ColumnClick += new ColumnClickEventHandler(ColumnClick);
                        if(!filterEnabled)
						    SC.LVCollection.Sort( (ListViewTextSort) Activator.CreateInstance(m_comparers[m_column], new Object[] { m_column, m_sortOrder == SortOrder.Ascending } ));
                        else
                            additionalStorage.Sort((ListViewTextSort) Activator.CreateInstance(m_comparers[m_column], new Object[] { m_column, m_sortOrder == SortOrder.Ascending } ));

						ShowHeaderIcon(m_list, m_column, m_sortOrder);
					}
				}

				else
				{
                    if(this.SortEnabled)
                    {						
                        m_list.ColumnClick -= new ColumnClickEventHandler(ColumnClick);
						m_list.ListViewItemSorter = null;
						ShowHeaderIcon(m_list, m_column, SortOrder.None);
					}
				}
			}
		}
		private void ColumnClick(object sender, ColumnClickEventArgs e)
		{
            try
            {
                this.Sort(e.Column);
            }
            catch(Exception)
            {
              //&&&
            }

		}
        private Int32 m_column;
		private SortOrder m_sortOrder;
        private EnhancedListView m_list;
        private Type[] m_comparers;
		private ImageList m_imgList;

		#region Graphics

		enum ArrowType { Ascending, Descending }
		Bitmap GetArrowBitmap(ArrowType type)
		{
			Bitmap bmp = new Bitmap(8, 8);
			Graphics gfx = Graphics.FromImage(bmp);

			Pen lightPen = SystemPens.ControlLightLight;
			Pen shadowPen = SystemPens.ControlDark;

			gfx.FillRectangle(System.Drawing.Brushes.Magenta, 0, 0, 8, 8);

			if(type == ArrowType.Ascending)		
			{
				gfx.DrawLine(lightPen, 0, 7, 7, 7);
				gfx.DrawLine(lightPen, 7, 7, 4, 0);
				gfx.DrawLine(shadowPen, 3, 0, 0, 7);
			}

			else if(type == ArrowType.Descending)
			{
				gfx.DrawLine(lightPen, 4, 7, 7, 0);
				gfx.DrawLine(shadowPen, 3, 7, 0, 0);
				gfx.DrawLine(shadowPen, 0, 0, 7, 0);
			}
			
			gfx.Dispose();

			return bmp;
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct HDITEM 
		{
			public Int32     mask; 
			public Int32     cxy; 	
			[MarshalAs(UnmanagedType.LPTStr)] 
			public String    pszText; 
			public IntPtr	 hbm; 
			public Int32     cchTextMax; 
			public Int32     fmt; 
			public Int32     lParam; 
			public Int32     iImage;
			public Int32     iOrder;
		};
		[DllImport("user32")]
		static extern IntPtr SendMessage(IntPtr Handle, Int32 msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32", EntryPoint="SendMessage")]
		static extern IntPtr SendMessage2(IntPtr Handle, Int32 msg, IntPtr wParam, ref HDITEM lParam);
		const Int32 HDI_WIDTH			= 0x0001;
		const Int32 HDI_HEIGHT			= HDI_WIDTH;
		const Int32 HDI_TEXT			= 0x0002;
		const Int32 HDI_FORMAT			= 0x0004;
		const Int32 HDI_LPARAM			= 0x0008;
		const Int32 HDI_BITMAP			= 0x0010;
		const Int32 HDI_IMAGE			= 0x0020;
		const Int32 HDI_DI_SETITEM		= 0x0040;
		const Int32 HDI_ORDER			= 0x0080;
		const Int32 HDI_FILTER			= 0x0100;		// 0x0500
		const Int32 HDF_LEFT			= 0x0000;
		const Int32 HDF_RIGHT			= 0x0001;
		const Int32 HDF_CENTER			= 0x0002;
		const Int32 HDF_JUSTIFYMASK		= 0x0003;
		const Int32 HDF_RTLREADING		= 0x0004;
		const Int32 HDF_OWNERDRAW		= 0x8000;
		const Int32 HDF_STRING			= 0x4000;
		const Int32 HDF_BITMAP			= 0x2000;
		const Int32 HDF_BITMAP_ON_RIGHT = 0x1000;
		const Int32 HDF_IMAGE			= 0x0800;
		const Int32 HDF_SORTUP			= 0x0400;		// 0x0501
		const Int32 HDF_SORTDOWN		= 0x0200;		// 0x0501
		const Int32 LVM_FIRST           = 0x1000;		// List messages
		const Int32 LVM_GETHEADER		= LVM_FIRST + 31;
		const Int32 HDM_FIRST           = 0x1200;		// Header messages
		const Int32 HDM_SETIMAGELIST	= HDM_FIRST + 8;
		const Int32 HDM_GETIMAGELIST    = HDM_FIRST + 9;
		const Int32 HDM_GETITEM			= HDM_FIRST + 11;
		const Int32 HDM_SETITEM			= HDM_FIRST + 12;
		
        private void ShowHeaderIcon(EnhancedListView list, int columnIndex, SortOrder sortOrder)
		{
			if(columnIndex < 0 || columnIndex >= list.Columns.Count)
				return;

            IntPtr hHeader =SendMessage(list.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

			ColumnHeader colHdr = list.Columns[columnIndex];

			HDITEM hd = new HDITEM();
			hd.mask = HDI_FORMAT;

			HorizontalAlignment align = colHdr.TextAlign;

			if(align == HorizontalAlignment.Left)
				hd.fmt = HDF_LEFT | HDF_STRING | HDF_BITMAP_ON_RIGHT;

			else if(align == HorizontalAlignment.Center)
				hd.fmt = HDF_CENTER | HDF_STRING | HDF_BITMAP_ON_RIGHT;

			else	// HorizontalAlignment.Right
				hd.fmt = HDF_RIGHT | HDF_STRING;
            
            if(s_useNativeArrows)
            {
                if(sortOrder == SortOrder.Ascending)
                    hd.fmt |= HDF_SORTUP;

                else if(sortOrder == SortOrder.Descending)
                    hd.fmt |= HDF_SORTDOWN;
            }
            else
            {
                hd.mask |= HDI_IMAGE;

                if (sortOrder != SortOrder.None)
                    hd.fmt |= HDF_IMAGE;

                hd.iImage = (int)sortOrder - 1;
                //check
                colHdr.ImageIndex = (sortOrder == SortOrder.Ascending)?0:1;
                
            }
            
          
			SendMessage2(hHeader, HDM_SETITEM, new IntPtr(columnIndex), ref hd);
		}
		private void SetHeaderImageList(EnhancedListView list, ImageList imgList)
		{
			IntPtr hHeader = SendMessage(list.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
			SendMessage(hHeader, HDM_SETIMAGELIST, IntPtr.Zero, imgList.Handle);
		}
        #endregion
        #region ComCtrl information
        [StructLayout(LayoutKind.Sequential)]
        private struct DLLVERSIONINFO
        {
            public int cbSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformID;
        } 
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string fileName);
        [DllImport("kernel32.dll", CharSet=CharSet.Ansi, ExactSpelling=true)]
        public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll")]
        static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("comctl32.dll")]
        static extern int DllGetVersion(ref DLLVERSIONINFO pdvi);
        static private bool ComCtlDllSupportsArrows()
        {
            IntPtr hModule = IntPtr.Zero;

            try
            {
                hModule = LoadLibrary("comctl32.dll");
                if(hModule != IntPtr.Zero)
                {
                    UIntPtr proc = GetProcAddress(hModule, "DllGetVersion");
                    if(proc == UIntPtr.Zero)    // Old versions don't support this method
                        return false;
                }

                DLLVERSIONINFO vi = new DLLVERSIONINFO();
                vi.cbSize = Marshal.SizeOf(typeof(DLLVERSIONINFO));

                DllGetVersion(ref vi);

                return vi.dwMajorVersion >= 6;
            }
            finally
            {
                if(hModule != IntPtr.Zero)
                    FreeLibrary(hModule);
            }
        }
		#endregion
        internal void EnableFilterMode(List<ListViewItem> tempStorage)
        {
            filterEnabled = true;
            additionalStorage = tempStorage;
        }

        internal void DisableFilterMode()
        {
            filterEnabled = false;
            additionalStorage = null;
        }

        internal void DisableSorting()
        {
            this.SortEnabled = false;
            m_list.ListViewItemSorter = null;
            ShowHeaderIcon(m_list, m_column, SortOrder.None);
        }
        #endregion

        
    }
}