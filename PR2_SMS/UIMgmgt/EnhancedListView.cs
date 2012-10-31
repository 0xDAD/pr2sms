using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PR2_SMS
{
    public delegate ListViewItem GetItemDelegate(int idx);

    public partial class EnhancedListView : ListView
    {
        public void SetCallback(GetItemDelegate del)
        {
            GetItem = del;
        }
        public EnhancedListView():base()
        {
            //InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            base.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.handler_RetrieveVirtualItem);
            base.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.handler_CacheVirtualItems);

        }
        GetItemDelegate GetItem = null;
        const int WM_USER = 0x0400;
        const int OCM__BASE = WM_USER + 0x1c00;
        const int WM_REFLECT = OCM__BASE;
        const int WM_NOTIFY = 0x004E;
        const int NM_CUSTOMDRAW = -12;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
      /*  protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_REFLECT | WM_NOTIFY:
                    switch (Marshal.ReadInt32(m.LParam, IntPtr.Size * 2)) // nmhdr->code
                    {
                        case NM_CUSTOMDRAW:
                            // Отвечаем винде, что нам не требуется CUSTOMDRAW.
                            m.Result = IntPtr.Zero; //CDRF_DODEFAULT
                            return;
                                              }

                    break;
            }
                       base.WndProc(ref m);

        }
        */
        private ListViewItem[] m_cache;
        private int m_firstItem;
        internal void ClearCache()
        {
            m_cache = null;
        }
        public void AddColumn(string cname)
        {
            if (!InvokeRequired)
                this.Columns.Add(cname);
            else
                this.Invoke((StringTransl)delegate(string cn) { this.Columns.Add(cname); },cname);                    
        }
        private void handler_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            if (m_cache != null && e.StartIndex >= m_firstItem && e.EndIndex <= m_firstItem + m_cache.Length)
                return;
            m_firstItem = e.StartIndex;
            int length = e.EndIndex - e.StartIndex + 1;
            m_cache = new ListViewItem[length];
            for (int i = 0; i < m_cache.Length; i++)
            {
                m_cache[i] = GetItem(m_firstItem + i);
            }
        }
        private void handler_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            //Вставить проверочку на пустую лвайку
            if (m_cache != null && e.ItemIndex >= m_firstItem && e.ItemIndex < m_firstItem + m_cache.Length)
            {
                e.Item = m_cache[e.ItemIndex - m_firstItem];
            }
            else
            {
                e.Item = GetItem(e.ItemIndex);
            }
        }
        

    }
}
