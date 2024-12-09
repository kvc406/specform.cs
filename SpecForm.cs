#define InflateTire

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace WinUb
{
    public partial class SpecForm : Form
    {
        //-------form --------------------
        //MForm mForm;			//UB メインフォーム
        public PassWd passWd;       //password Form
        public SpecCsv specCsv;     //規格をCSVファイルにセーブ
        //------- spec -------------------------------
        public int s_ModelNo;	//規格編集model No.
        public Spec spec;		//編集中の規格
        private Spec[] specAll;	//全ての規格
        private Spec SpTmp;     //copy paste の１次的な保持
        public int CurRow;

        //----unit -----------------------
        public static Unit_t dbUnit;          //up low unit
        public static Unit_t msUnit;          //static unit
        public static Unit_t mcUnit;          //couple unit

        private Unit_t s_uUnit;	//規格編集中の型番の UF unit
        private Unit_t s_dbUnit = new Unit_t();  //                   dynamic unit  
        private Unit_t s_msUnit = new Unit_t();   //                   static unit
        private Unit_t s_mcUnit = new Unit_t();   //                   couple unit

        public int ModelNo;

        private bool F1key = true; //false; //ファンクションキーの有効無効
        private bool F2key = false;
        private bool F3key = false;
        private bool F4key = false;
        private bool F5key = false;
        private bool F6key = false;
        private bool F7key = false;
        private bool F8key = false;
        private bool F9key = false;
        private bool F10key = false;
        private bool F11key = false;
        private bool F12key = false;
        //---------------------------------------------------------

        //public SpecForm()
        //{
        //    InitializeComponent();
        //}

        public SpecForm()
        {
            //
            // Windows フォーム デザイナ サポートに必要です。
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
            //
            //			specAll = mForm.specAll;

        }

        /// <summary>
        /// 使用されているリソースに後処理を実行します。
        /// </summary>

        private void SpecForm_Load(object sender, System.EventArgs e)
        {
            string CurDir = Directory.GetCurrentDirectory();
#if InflateTire
            groupRimDt.Visible = false;
#else
            groupRimDt.Visible = true;
#endif
            //Sys.Init();     //sysconfのセット
            
            //Unit_Set();
            Unit.UnitSet();
            Sys.ConfigLoad();

            spec = new Spec();
            //m_Spec = new Spec();
            specAll = new Spec[Sys.ModelMax];
            for (int i = 0; i < Sys.ModelMax; i++)
            {
                specAll[i] = new Spec();
            }
            spec.DiskRead(specAll);     //規格を全てロード
            SpecException(specAll);     //規格の例外処理

            grdModelSet();          //model 一覧表示
            s_ModelNo = 0;          //規格モデルＮｏ．１に設定
            updownModelNo.Maximum = Sys.ModelMax;   //モデルＮｏ．最大値設定
            TabSpecSet(s_ModelNo);  //そのモデルの規格全てセット
            dataGridModel.CurrentRowIndex = ModelNo;
            s_ModelNo = ModelNo;
            TabSpecSet(s_ModelNo);  //そのモデルの規格全てセット
            TabSpecUseSet();        //規格設定の有効無効をセット
            passWd = new PassWd();
            specCsv = new SpecCsv();
            FkeySet();              //ボタン、Fキー有効無効をセット
            
        }

        //
        //アプリケーションの終了
        //
        public void AppEnd()
        {
            Application.Exit();
        }

        //----- event----------------------------
        private void btnModelSet_Click(object sender, EventArgs e)
        {
            modelSetClick();
            //int mno;
            //mno = Convert.ToInt32(updownModelNo.Text) - 1;
            //ChangModelNo(s_ModelNo, mno);   //編集中の規格をｇｅｔし新しいモデルの規格をセット
            ////dataGridModel.CurrentCell.RowNumber = mno;　//グリッドのセルＮｏ．を選択
            //dataGridModel.CurrentRowIndex = mno;
            //s_ModelNo = mno;
        }

        private void btnModelCopy_Click(object sender, EventArgs e)
        {
            modelCopyClick();
            ////TabSpecGet(s_ModelNo);
            //SpTmp = mForm.specAll[s_ModelNo];
            //btnModelPaste.Enabled = true;

        }

        private void btnModelPaste_Click(object sender, EventArgs e)
        {
            modelPasteClick();
            //mForm.specAll[s_ModelNo] = SpTmp.Copy();
            //TabSpecSet(s_ModelNo);
            //grdModelSet();
        }

        private void btnModelDelete_Click(object sender, EventArgs e)
        {
            modelDelClick();
            //Spec specNull = new Spec();
            //mForm.specAll[s_ModelNo] = specNull;    //規格の消去
            //TabSpecSet(s_ModelNo);
            //grdModelSet();
        }

        private void btnSpecCsv_Click(object sender, EventArgs e)
        {
            specCsv.SpecCsvSave(specAll);
        }

        private void btnPassWord_Click(object sender, EventArgs e)
        {
            passWd.ShowDialog();
            if (PassWd.PasswordOK != 0)
            {
                //btnSpec.Enabled = true;
            }
            else
            {
                //btnSpec.Enabled = false;
            }
            FkeySet();
            TabSpecUseSet();
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            saveClick();
            AppEnd();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            AppEnd();
            //this.Hide();
        }

        //------------------------------------------------------------
        private void modelSetClick()
        {
            int mno;
            mno = Convert.ToInt32(updownModelNo.Text) - 1;
            ChangModelNo(s_ModelNo, mno);   //編集中の規格をｇｅｔし新しいモデルの規格をセット
            //dataGridModel.CurrentCell.RowNumber = mno;　//グリッドのセルＮｏ．を選択
            dataGridModel.CurrentRowIndex = mno;
            s_ModelNo = mno;
        }

        private void modelCopyClick()
        {
            //TabSpecGet(s_ModelNo);
            SpTmp = specAll[s_ModelNo];
            btnModelPaste.Enabled = true;
        }

        private void modelPasteClick()
        {
            specAll[s_ModelNo] = SpTmp.Copy();
            TabSpecSet(s_ModelNo);
            grdModelSet();
        }

        private void modelDelClick()
        {
            Spec specNull = new Spec();
            specAll[s_ModelNo] = specNull;    //規格の消去
            TabSpecSet(s_ModelNo);
            grdModelSet();
        }

        private void saveClick()
        {
            TabSpecGet(s_ModelNo);		        //編集中の規格をget
            spec.DiskWrite(specAll);
            //Spec.Serialize(specAll);		//全ての規格をセーブ
            this.Hide();
        }
        //-----------------------------------------------------------

        private void FkeySet()
        {
            switch (PassWd.PasswordOK)
            {
                case 0:
                    //btnSpec.Enabled = false; F1key = false;
                    btnModelCopy.Enabled = false; F2key = false;
                    btnModelPaste.Enabled = false; F3key = false;
                    btnModelDelete.Enabled = false; F4key = false;
                    btnSpecCsv.Enabled = false; F7key = false;
                    btnSave.Enabled = false; F9key = false;
                    //btnCancel.Enabled = true; F10key = true;
                    break;

                case 1:
                    //btnSpec.Enabled = false; F1key = false;
                    btnModelCopy.Enabled = true; F2key = true;
                    btnModelPaste.Enabled = true; F3key = true;
                    btnModelDelete.Enabled = true; F4key = true;
                    btnSpecCsv.Enabled = true; F7key = true;
                    btnSave.Enabled = true; F9key = true;
                    //btnCancel.Enabled = true; F10key = true;
                    break;

                case 2:
                    //btnSpec.Enabled = false; F1key = false;
                    btnModelCopy.Enabled = true; F2key = true;
                    btnModelPaste.Enabled = true; F3key = true;
                    btnModelDelete.Enabled = true; F4key = true;
                    btnSpecCsv.Enabled = true; F7key = true;
                    btnSave.Enabled = true; F9key = true;
                    //btnCancel.Enabled = true; F10key = true;
                    break;

            }
        }

        private void dataGridModel_CurrentCellChanged(object sender, System.EventArgs e)
        {
            int Row = dataGridModel.CurrentCell.RowNumber;
            //int b = dataGridModel[dataGridModel.CurrentCell];	
            TabSpecGet(s_ModelNo);		//編集中の規格をget
            s_ModelNo = Row;
            updownModelNo.Text = (s_ModelNo + 1).ToString();
            TabSpecSet(s_ModelNo);		//次の規格をセット
        }

        private void ChangModelNo(int oModelNo, int ModelNo)
        {
            TabSpecGet(oModelNo);		//編集中の規格をget
            s_ModelNo = ModelNo;
            updownModelNo.Text = (ModelNo + 1).ToString();
            TabSpecSet(ModelNo);		//次の規格をセット
        }

        //model 一覧表示
        private void grdModelSet()
        {
            DataSet ds = new DataSet("MODEL");
            //DataGridTableStyle tstyle;
            //DataGridColumnStyle cstyle1, cstyle2, cstyle3, cstyle4, cstyle5;
            DataTable dt;
            DataColumn[] dc = new DataColumn[1];

            dt = ds.Tables.Add("m_MODEL");
            dc[0] = dt.Columns.Add("No.", Type.GetType("System.String"));
            dt.PrimaryKey = dc;
            dt.Columns.Add("Size code", Type.GetType("System.String"));
            //			dt.Columns..
            dt.Columns.Add("Tire name", Type.GetType("System.String"));
            dt.Columns.Add("Bead Dia.", Type.GetType("System.String"));
            dt.Columns.Add("Bead Width", Type.GetType("System.String"));

            int n;
            for (int i = 0; i < Sys.ModelMax; i++)
            {
                n = i + 1;
                dt.Rows.Add(new object[] {n.ToString(), 
											 specAll[i].SizeCode, 
											 specAll[i].ModelName, 
											 specAll[i].size.Bdia.ToString(), 
											 specAll[i].size.Haba.ToString()});
            }

            /*                dataGridModel.RowHeaderWidth = 15;
                            tstyle = new DataGridTableStyle();
                            tstyle.MappingName = "m_MODEL";
                            dataGridModel.TableStyles.Add(tstyle);

                            cstyle1 = new DataGridTextBoxColumn();
                            cstyle1.MappingName = "No.";
                            cstyle1.Width = 30;
                            tstyle.GridColumnStyles.Add(cstyle1);

                            cstyle2 = new DataGridTextBoxColumn();
                            cstyle2.MappingName = "Size Code";
                            cstyle2.Width = 40;
                            tstyle.GridColumnStyles.Add(cstyle2);

                            cstyle3 = new DataGridTextBoxColumn();
                            cstyle3.MappingName = "Tire Name";
                            cstyle3.Width = 150;
                            tstyle.GridColumnStyles.Add(cstyle3);

                            cstyle4 = new DataGridTextBoxColumn();
                            cstyle4.MappingName = "Bead Dia.";
                            cstyle4.Width = 40;
                            tstyle.GridColumnStyles.Add(cstyle4);

                            cstyle5 = new DataGridTextBoxColumn();
                            cstyle5.MappingName = "Bead Width";
                            cstyle5.Width = 40;
                            tstyle.GridColumnStyles.Add(cstyle5);
            */
            dataGridModel.SetDataBinding(ds, "m_MODEL");

        }

        //------size code -------
        private void TabModelSet(Spec spec)
        {
            lblSizeCodeModelNo.Text = (s_ModelNo + 1).ToString();
            txtSizeCode.Text = spec.SizeCode;	//.ToString();
            txtModel.Text = spec.ModelName;		//.ToString();
            txtBdia.Text = (spec.size.Bdia * Unit.lUnit[1, 0].Ka).ToString();
            txtBeadHaba.Text = (spec.size.Haba * Unit.lUnit[1, 0].Ka).ToString();
            txtOdia.Text = spec.size.Odia.ToString();
            txtRimNo.Text = spec.size.RimNo.ToString();
            txtTireHaba.Text = spec.size.TireHaba.ToString();

        }
        private void TabModelGet(Spec spec)
        {
            spec.ModelNo = lblSizeCodeModelNo.Text;
            spec.SizeCode = txtSizeCode.Text;	//.ToCharArray();
            spec.ModelName = txtModel.Text;	//.ToCharArray();
            spec.size.Bdia = Convert.ToSingle(txtBdia.Text) / Unit.lUnit[1, 0].Ka;
            spec.size.Haba = Convert.ToSingle(txtBeadHaba.Text) / Unit.lUnit[1, 0].Ka;
            spec.size.Odia = Convert.ToSingle(txtOdia.Text);
            spec.size.RimNo = Convert.ToInt32(txtRimNo.Text);
            spec.size.TireHaba = Convert.ToSingle(txtTireHaba.Text);

        }
        //======Coef.Set ================
        private void TabCoefSet(Spec spec)
        {
            float k = s_uUnit.Ka;
            lblCoefModelNo.Text = (s_ModelNo + 1).ToString();
            txtCoRFVKa.Text = spec.coUf.RFV.A.ToString("###0.00");
            txtCoRFVKb.Text = (spec.coUf.RFV.B * k).ToString("###0.00");
            txtCoRFVrKa.Text = spec.coUf.RFVr.A.ToString("###0.00");
            txtCoRFVrKb.Text = (spec.coUf.RFVr.B * k).ToString("###0.00");
            txtCoRHKa.Text = spec.coUf.RH.A.ToString("###0.00");
            txtCoRHKb.Text = (spec.coUf.RH.B * k).ToString("###0.00");
            txtCoRHrKa.Text = spec.coUf.RHr.A.ToString("###0.00");
            txtCoRHrKb.Text = (spec.coUf.RHr.B * k).ToString("###0.00");
            txtCoLFVKa.Text = spec.coUf.LFV.A.ToString("###0.00");
            txtCoLFVKb.Text = (spec.coUf.LFV.B * k).ToString("###0.00");
            txtCoLFVrKa.Text = spec.coUf.LFVr.A.ToString("###0.00");
            txtCoLFVrKb.Text = (spec.coUf.LFVr.B * k).ToString("###0.00");
            txtCoLHKa.Text = spec.coUf.LH.A.ToString("###0.00");
            txtCoLHKb.Text = (spec.coUf.LH.B * k).ToString("###0.00");
            txtCoLHrKa.Text = spec.coUf.LHr.A.ToString("###0.00");
            txtCoLHrKb.Text = (spec.coUf.LHr.B * k).ToString("###0.00");
            txtCoLFDKa.Text = spec.coUf.LFD.A.ToString("###0.00");
            txtCoLFDKb.Text = (spec.coUf.LFD.B * k).ToString("###0.00");
            txtCoLFDrKa.Text = spec.coUf.LFDr.A.ToString("###0.00");
            txtCoLFDrKb.Text = (spec.coUf.LFDr.B * k).ToString("###0.00");
            txtCoCONKa.Text = spec.coUf.CON.A.ToString("###0.00");
            txtCoCONKb.Text = (spec.coUf.CON.B * k).ToString("###0.00");
            txtCoPLYKa.Text = spec.coUf.PLY.A.ToString("###0.00");
            txtCoPLYKb.Text = (spec.coUf.PLY.B * k).ToString("###0.00");

            txtCoRTKa.Text = spec.coRo.RT.A.ToString("###0.00");
            txtCoRTKb.Text = spec.coRo.RT.B.ToString("###0.00");
            txtCoRCKa.Text = spec.coRo.RC.A.ToString("###0.00");
            txtCoRCKb.Text = spec.coRo.RC.B.ToString("###0.00");
            txtCoRBKa.Text = spec.coRo.RB.A.ToString("###0.00");
            txtCoRBKb.Text = spec.coRo.RB.B.ToString("###0.00");
            txtCoLTKa.Text = spec.coRo.LT.A.ToString("###0.00");
            txtCoLTKb.Text = spec.coRo.LT.B.ToString("###0.00");
            txtCoLBKa.Text = spec.coRo.LB.A.ToString("###0.00");
            txtCoLBKb.Text = spec.coRo.LB.B.ToString("###0.00");

            txtCoBulgTKa.Text = spec.coRo.BulgT.A.ToString("###0.00");
            txtCoBulgTKb.Text = spec.coRo.BulgT.B.ToString("###0.00");
            txtCoDentTKa.Text = spec.coRo.DentT.A.ToString("###0.00");
            txtCoDentTKb.Text = spec.coRo.DentT.B.ToString("###0.00");
            txtCoBulgBKa.Text = spec.coRo.BulgB.A.ToString("###0.00");
            txtCoBulgBKb.Text = spec.coRo.BulgB.B.ToString("###0.00");
            txtCoDentBKa.Text = spec.coRo.DentB.A.ToString("###0.00");
            txtCoDentBKb.Text = spec.coRo.DentB.B.ToString("###0.00");

            txtCoMLKa.Text = spec.coBal.ML.A.ToString("###0.00");
            txtCoMLKb.Text = spec.coBal.ML.B.ToString("###0.00");
            txtCoMRKa.Text = spec.coBal.MR.A.ToString("###0.00");
            txtCoMRKb.Text = spec.coBal.MR.B.ToString("###0.00");
            
        }

        private void TabCoefGet(Spec spec)
        {
            float k = s_uUnit.Ka;
            spec.coUf.RFV.A = Convert.ToSingle(txtCoRFVKa.Text);
            spec.coUf.RFV.B = Convert.ToSingle(txtCoRFVKb.Text) / k;
            spec.coUf.RFVr.A = Convert.ToSingle(txtCoRFVrKa.Text);
            spec.coUf.RFVr.B = Convert.ToSingle(txtCoRFVrKb.Text) / k;
            spec.coUf.RH.A = Convert.ToSingle(txtCoRHKa.Text);
            spec.coUf.RH.B = Convert.ToSingle(txtCoRHKb.Text) / k;
            spec.coUf.RHr.A = Convert.ToSingle(txtCoRHrKa.Text);
            spec.coUf.RHr.B = Convert.ToSingle(txtCoRHrKb.Text) / k;
            spec.coUf.LFV.A = Convert.ToSingle(txtCoLFVKa.Text);
            spec.coUf.LFV.B = Convert.ToSingle(txtCoLFVKb.Text) / k;
            spec.coUf.LFVr.A = Convert.ToSingle(txtCoLFVrKa.Text);
            spec.coUf.LFVr.B = Convert.ToSingle(txtCoLFVrKb.Text) / k;
            spec.coUf.LH.A = Convert.ToSingle(txtCoLHKa.Text);
            spec.coUf.LH.B = Convert.ToSingle(txtCoLHKb.Text) / k;
            spec.coUf.LHr.A = Convert.ToSingle(txtCoLHrKa.Text);
            spec.coUf.LHr.B = Convert.ToSingle(txtCoLHrKb.Text) / k;
            spec.coUf.LFD.A = Convert.ToSingle(txtCoLFDKa.Text);
            spec.coUf.LFD.B = Convert.ToSingle(txtCoLFDKb.Text) / k;
            spec.coUf.LFDr.A = Convert.ToSingle(txtCoLFDrKa.Text);
            spec.coUf.LFDr.B = Convert.ToSingle(txtCoLFDrKb.Text) / k;
            spec.coUf.CON.A = Convert.ToSingle(txtCoCONKa.Text);
            spec.coUf.CON.B = Convert.ToSingle(txtCoCONKb.Text) / k;
            spec.coUf.PLY.A = Convert.ToSingle(txtCoPLYKa.Text);
            spec.coUf.PLY.B = Convert.ToSingle(txtCoPLYKb.Text) / k;

            spec.coRo.RT.A = Convert.ToSingle(txtCoRTKa.Text);
            spec.coRo.RT.B = Convert.ToSingle(txtCoRTKb.Text);
            spec.coRo.RC.A = Convert.ToSingle(txtCoRCKa.Text);
            spec.coRo.RC.B = Convert.ToSingle(txtCoRCKb.Text);
            spec.coRo.RB.A = Convert.ToSingle(txtCoRBKa.Text);
            spec.coRo.RB.B = Convert.ToSingle(txtCoRBKb.Text);
            spec.coRo.LT.A = Convert.ToSingle(txtCoLTKa.Text);
            spec.coRo.LT.B = Convert.ToSingle(txtCoLTKb.Text);
            spec.coRo.LB.A = Convert.ToSingle(txtCoLBKa.Text);
            spec.coRo.LB.B = Convert.ToSingle(txtCoLBKb.Text);
            spec.coRo.BulgT.A = Convert.ToSingle(txtCoBulgTKa.Text);
            spec.coRo.BulgT.B = Convert.ToSingle(txtCoBulgTKb.Text);
            spec.coRo.DentT.A = Convert.ToSingle(txtCoDentTKa.Text);
            spec.coRo.DentT.B = Convert.ToSingle(txtCoDentTKb.Text);
            spec.coRo.BulgB.A = Convert.ToSingle(txtCoBulgBKa.Text);
            spec.coRo.BulgB.B = Convert.ToSingle(txtCoBulgBKb.Text);
            spec.coRo.DentB.A = Convert.ToSingle(txtCoDentBKa.Text);
            spec.coRo.DentB.B = Convert.ToSingle(txtCoDentBKb.Text);

            spec.coBal.ML.A = Convert.ToSingle(txtCoMLKa.Text);
            spec.coBal.ML.B = Convert.ToSingle(txtCoMLKb.Text);
            spec.coBal.MR.A = Convert.ToSingle(txtCoMRKa.Text);
            spec.coBal.MR.B = Convert.ToSingle(txtCoMRKb.Text);
        }

        private void TabUfhCoefSet(Spec spec)
        {
            float k = s_uUnit.Ka;
            try
            {
                txtCoHRFVKa.Text = spec.coUfh.RFV[0].A.ToString("###0.00");
                txtCoHRFVKb.Text = (spec.coUfh.RFV[0].B * k).ToString("###0.00");
                txtCoHRHKa.Text = spec.coUfh.RFV[1].A.ToString("###0.00");
                txtCoHRHKb.Text = (spec.coUfh.RFV[1].B * k).ToString("###0.00");
                txtCoHR2HKa.Text = spec.coUfh.RFV[2].A.ToString("###0.00");
                txtCoHR2HKb.Text = (spec.coUfh.RFV[2].B * k).ToString("###0.00");
                txtCoHR3HKa.Text = spec.coUfh.RFV[3].A.ToString("###0.00");
                txtCoHR3HKb.Text = (spec.coUfh.RFV[3].B * k).ToString("###0.00");

                txtCoHLFVKa.Text = spec.coUfh.LFV[0].A.ToString("###0.00");
                txtCoHLFVKb.Text = (spec.coUfh.LFV[0].B * k).ToString("###0.00");
                txtCoHLHKa.Text = spec.coUfh.LFV[1].A.ToString("###0.00");
                txtCoHLHKb.Text = (spec.coUfh.LFV[1].B * k).ToString("###0.00");
                txtCoHL2HKa.Text = spec.coUfh.LFV[2].A.ToString("###0.00");
                txtCoHL2HKb.Text = (spec.coUfh.LFV[2].B * k).ToString("###0.00");
                txtCoHL3HKa.Text = spec.coUfh.LFV[3].A.ToString("###0.00");
                txtCoHL3HKb.Text = (spec.coUfh.LFV[3].B * k).ToString("###0.00");

                txtCoHTFVKa.Text = spec.coUfh.TFV[0].A.ToString("###0.00");
                txtCoHTFVKb.Text = (spec.coUfh.TFV[0].B * k).ToString("###0.00");
                txtCoHTHKa.Text = spec.coUfh.TFV[1].A.ToString("###0.00");
                txtCoHTHKb.Text = (spec.coUfh.TFV[1].B * k).ToString("###0.00");
                txtCoHT2HKa.Text = spec.coUfh.TFV[2].A.ToString("###0.00");
                txtCoHT2HKb.Text = (spec.coUfh.TFV[2].B * k).ToString("###0.00");
                txtCoHT3HKa.Text = spec.coUfh.TFV[3].A.ToString("###0.00");
                txtCoHT3HKb.Text = (spec.coUfh.TFV[3].B * k).ToString("###0.00");
            }
            catch
            {
                spec.coUfh = new CoUfh();
                //return;
            }
        }

        private void TabUfhCoefGet(Spec spec)
        {
            try
            {
                float k = s_uUnit.Ka;
                spec.coUfh.RFV[0].A = Convert.ToSingle(txtCoHRFVKa.Text);
                spec.coUfh.RFV[0].B = Convert.ToSingle(txtCoHRFVKb.Text) / k;
                spec.coUfh.RFV[1].A = Convert.ToSingle(txtCoHRHKa.Text);
                spec.coUfh.RFV[1].B = Convert.ToSingle(txtCoHRHKb.Text) / k;
                spec.coUfh.RFV[2].A = Convert.ToSingle(txtCoHR2HKa.Text);
                spec.coUfh.RFV[2].B = Convert.ToSingle(txtCoHR2HKb.Text) / k;
                spec.coUfh.RFV[3].A = Convert.ToSingle(txtCoHR3HKa.Text);
                spec.coUfh.RFV[3].B = Convert.ToSingle(txtCoHR3HKb.Text) / k;

                spec.coUfh.LFV[0].A = Convert.ToSingle(txtCoHLFVKa.Text);
                spec.coUfh.LFV[0].B = Convert.ToSingle(txtCoHLFVKb.Text) / k;
                spec.coUfh.LFV[1].A = Convert.ToSingle(txtCoHLHKa.Text);
                spec.coUfh.LFV[1].B = Convert.ToSingle(txtCoHLHKb.Text) / k;
                spec.coUfh.LFV[2].A = Convert.ToSingle(txtCoHL2HKa.Text);
                spec.coUfh.LFV[2].B = Convert.ToSingle(txtCoHL2HKb.Text) / k;
                spec.coUfh.LFV[3].A = Convert.ToSingle(txtCoHL3HKa.Text);
                spec.coUfh.LFV[3].B = Convert.ToSingle(txtCoHL3HKb.Text) / k;

                spec.coUfh.TFV[0].A = Convert.ToSingle(txtCoHTFVKa.Text);
                spec.coUfh.TFV[0].B = Convert.ToSingle(txtCoHTFVKb.Text) / k;
                spec.coUfh.TFV[1].A = Convert.ToSingle(txtCoHTHKa.Text);
                spec.coUfh.TFV[1].B = Convert.ToSingle(txtCoHTHKb.Text) / k;
                spec.coUfh.TFV[2].A = Convert.ToSingle(txtCoHT2HKa.Text);
                spec.coUfh.TFV[2].B = Convert.ToSingle(txtCoHT2HKb.Text) / k;
                spec.coUfh.TFV[3].A = Convert.ToSingle(txtCoHT3HKa.Text);
                spec.coUfh.TFV[3].B = Convert.ToSingle(txtCoHT3HKb.Text) / k;

                for (int i = 4; i < 11; i++)
                {
                    spec.coUfh.RFV[i].A = 1;
                    spec.coUfh.RFV[i].B = 0;
                    spec.coUfh.LFV[i].A = 1;
                    spec.coUfh.LFV[i].B = 0;
                    spec.coUfh.TFV[i].A = 1;
                    spec.coUfh.TFV[i].B = 0;
                }
            }
            catch
            {
                spec.coUfh = new CoUfh();
            }

        }
        //======uf rank =================
        private void TabUfRankSet(Spec spec)
        {
            try
            {
                float k = s_uUnit.Ka;
                lblUfRankModelNo.Text = (s_ModelNo + 1).ToString();
                txtRFV_A.Text = (spec.ufm.RFV[0] * k).ToString("###0.00");
                txtRFV_B.Text = (spec.ufm.RFV[1] * k).ToString("###0.00");
                txtRFV_C.Text = (spec.ufm.RFV[2] * k).ToString("###0.00");
                txtRFV_D.Text = (spec.ufm.RFV[3] * k).ToString("###0.00");

                txtRH_A.Text = (spec.ufm.RH[0] * k).ToString("###0.00");
                txtRH_B.Text = (spec.ufm.RH[1] * k).ToString("###0.00");
                txtRH_C.Text = (spec.ufm.RH[2] * k).ToString("###0.00");
                txtRH_D.Text = (spec.ufm.RH[3] * k).ToString("###0.00");

                txtRH2_A.Text = (spec.ufm.RH2[0] * k).ToString("###0.00");
                txtRH2_B.Text = (spec.ufm.RH2[1] * k).ToString("###0.00");
                txtRH2_C.Text = (spec.ufm.RH2[2] * k).ToString("###0.00");
                txtRH2_D.Text = (spec.ufm.RH2[3] * k).ToString("###0.00");

                txtRH3_A.Text = (spec.ufm.RH3[0] * k).ToString("###0.00");
                txtRH3_B.Text = (spec.ufm.RH3[1] * k).ToString("###0.00");
                txtRH3_C.Text = (spec.ufm.RH3[2] * k).ToString("###0.00");
                txtRH3_D.Text = (spec.ufm.RH3[3] * k).ToString("###0.00");

                txtRH4_A.Text = (spec.ufm.RH4[0] * k).ToString("###0.00");
                txtRH4_B.Text = (spec.ufm.RH4[1] * k).ToString("###0.00");
                txtRH4_C.Text = (spec.ufm.RH4[2] * k).ToString("###0.00");
                txtRH4_D.Text = (spec.ufm.RH4[3] * k).ToString("###0.00");

                txtRH5_A.Text = (spec.ufm.RH5[0] * k).ToString("###0.00");
                txtRH5_B.Text = (spec.ufm.RH5[1] * k).ToString("###0.00");
                txtRH5_C.Text = (spec.ufm.RH5[2] * k).ToString("###0.00");
                txtRH5_D.Text = (spec.ufm.RH5[3] * k).ToString("###0.00");

                txtRH6_A.Text = (spec.ufm.RH6[0] * k).ToString("###0.00");
                txtRH6_B.Text = (spec.ufm.RH6[1] * k).ToString("###0.00");
                txtRH6_C.Text = (spec.ufm.RH6[2] * k).ToString("###0.00");
                txtRH6_D.Text = (spec.ufm.RH6[3] * k).ToString("###0.00");

                txtRH7_A.Text = (spec.ufm.RH7[0] * k).ToString("###0.00");
                txtRH7_B.Text = (spec.ufm.RH7[1] * k).ToString("###0.00");
                txtRH7_C.Text = (spec.ufm.RH7[2] * k).ToString("###0.00");
                txtRH7_D.Text = (spec.ufm.RH7[3] * k).ToString("###0.00");

                txtRH8_A.Text = (spec.ufm.RH8[0] * k).ToString("###0.00");
                txtRH8_B.Text = (spec.ufm.RH8[1] * k).ToString("###0.00");
                txtRH8_C.Text = (spec.ufm.RH8[2] * k).ToString("###0.00");
                txtRH8_D.Text = (spec.ufm.RH8[3] * k).ToString("###0.00");

                txtRH9_A.Text = (spec.ufm.RH9[0] * k).ToString("###0.00");
                txtRH9_B.Text = (spec.ufm.RH9[1] * k).ToString("###0.00");
                txtRH9_C.Text = (spec.ufm.RH9[2] * k).ToString("###0.00");
                txtRH9_D.Text = (spec.ufm.RH9[3] * k).ToString("###0.00");

                txtRH10_A.Text = (spec.ufm.RH10[0] * k).ToString("###0.00");
                txtRH10_B.Text = (spec.ufm.RH10[1] * k).ToString("###0.00");
                txtRH10_C.Text = (spec.ufm.RH10[2] * k).ToString("###0.00");
                txtRH10_D.Text = (spec.ufm.RH10[3] * k).ToString("###0.00");

                txtLFV_A.Text = (spec.ufm.LFV[0] * k).ToString("###0.00");
                txtLFV_B.Text = (spec.ufm.LFV[1] * k).ToString("###0.00");
                txtLFV_C.Text = (spec.ufm.LFV[2] * k).ToString("###0.00");
                txtLFV_D.Text = (spec.ufm.LFV[3] * k).ToString("###0.00");

                txtLH_A.Text = (spec.ufm.LH[0] * k).ToString("###0.00");
                txtLH_B.Text = (spec.ufm.LH[1] * k).ToString("###0.00");
                txtLH_C.Text = (spec.ufm.LH[2] * k).ToString("###0.00");
                txtLH_D.Text = (spec.ufm.LH[3] * k).ToString("###0.00");

                txtLH2_A.Text = (spec.ufm.LH2[0] * k).ToString("###0.00");
                txtLH2_B.Text = (spec.ufm.LH2[1] * k).ToString("###0.00");
                txtLH2_C.Text = (spec.ufm.LH2[2] * k).ToString("###0.00");
                txtLH2_D.Text = (spec.ufm.LH2[3] * k).ToString("###0.00");

                txtLH3_A.Text = (spec.ufm.LH3[0] * k).ToString("###0.00");
                txtLH3_B.Text = (spec.ufm.LH3[1] * k).ToString("###0.00");
                txtLH3_C.Text = (spec.ufm.LH3[2] * k).ToString("###0.00");
                txtLH3_D.Text = (spec.ufm.LH3[3] * k).ToString("###0.00");

                txtLH4_A.Text = (spec.ufm.LH4[0] * k).ToString("###0.00");
                txtLH4_B.Text = (spec.ufm.LH4[1] * k).ToString("###0.00");
                txtLH4_C.Text = (spec.ufm.LH4[2] * k).ToString("###0.00");
                txtLH4_D.Text = (spec.ufm.LH4[3] * k).ToString("###0.00");

                txtLH5_A.Text = (spec.ufm.LH5[0] * k).ToString("###0.00");
                txtLH5_B.Text = (spec.ufm.LH5[1] * k).ToString("###0.00");
                txtLH5_C.Text = (spec.ufm.LH5[2] * k).ToString("###0.00");
                txtLH5_D.Text = (spec.ufm.LH5[3] * k).ToString("###0.00");

                txtLH6_A.Text = (spec.ufm.LH6[0] * k).ToString("###0.00");
                txtLH6_B.Text = (spec.ufm.LH6[1] * k).ToString("###0.00");
                txtLH6_C.Text = (spec.ufm.LH6[2] * k).ToString("###0.00");
                txtLH6_D.Text = (spec.ufm.LH6[3] * k).ToString("###0.00");

                txtLH7_A.Text = (spec.ufm.LH7[0] * k).ToString("###0.00");
                txtLH7_B.Text = (spec.ufm.LH7[1] * k).ToString("###0.00");
                txtLH7_C.Text = (spec.ufm.LH7[2] * k).ToString("###0.00");
                txtLH7_D.Text = (spec.ufm.LH7[3] * k).ToString("###0.00");

                txtLH8_A.Text = (spec.ufm.LH8[0] * k).ToString("###0.00");
                txtLH8_B.Text = (spec.ufm.LH8[1] * k).ToString("###0.00");
                txtLH8_C.Text = (spec.ufm.LH8[2] * k).ToString("###0.00");
                txtLH8_D.Text = (spec.ufm.LH8[3] * k).ToString("###0.00");

                txtLH9_A.Text = (spec.ufm.LH9[0] * k).ToString("###0.00");
                txtLH9_B.Text = (spec.ufm.LH9[1] * k).ToString("###0.00");
                txtLH9_C.Text = (spec.ufm.LH9[2] * k).ToString("###0.00");
                txtLH9_D.Text = (spec.ufm.LH9[3] * k).ToString("###0.00");

                txtLH10_A.Text = (spec.ufm.LH10[0] * k).ToString("###0.00");
                txtLH10_B.Text = (spec.ufm.LH10[1] * k).ToString("###0.00");
                txtLH10_C.Text = (spec.ufm.LH10[2] * k).ToString("###0.00");
                txtLH10_D.Text = (spec.ufm.LH10[3] * k).ToString("###0.00");

                txtRFVr_A.Text = (spec.ufm.RFVr[0] * k).ToString("###0.00");
                txtRFVr_B.Text = (spec.ufm.RFVr[1] * k).ToString("###0.00");
                txtRFVr_C.Text = (spec.ufm.RFVr[2] * k).ToString("###0.00");
                txtRFVr_D.Text = (spec.ufm.RFVr[3] * k).ToString("###0.00");

                txtRHr_A.Text = (spec.ufm.RHr[0] * k).ToString("###0.00");
                txtRHr_B.Text = (spec.ufm.RHr[1] * k).ToString("###0.00");
                txtRHr_C.Text = (spec.ufm.RHr[2] * k).ToString("###0.00");
                txtRHr_D.Text = (spec.ufm.RHr[3] * k).ToString("###0.00");

                txtRH2r_A.Text = (spec.ufm.RH2r[0] * k).ToString("###0.00");
                txtRH2r_B.Text = (spec.ufm.RH2r[1] * k).ToString("###0.00");
                txtRH2r_C.Text = (spec.ufm.RH2r[2] * k).ToString("###0.00");
                txtRH2r_D.Text = (spec.ufm.RH2r[3] * k).ToString("###0.00");

                txtRH3r_A.Text = (spec.ufm.RH3r[0] * k).ToString("###0.00");
                txtRH3r_B.Text = (spec.ufm.RH3r[1] * k).ToString("###0.00");
                txtRH3r_C.Text = (spec.ufm.RH3r[2] * k).ToString("###0.00");
                txtRH3r_D.Text = (spec.ufm.RH3r[3] * k).ToString("###0.00");

                txtRH4r_A.Text = (spec.ufm.RH4r[0] * k).ToString("###0.00");
                txtRH4r_B.Text = (spec.ufm.RH4r[1] * k).ToString("###0.00");
                txtRH4r_C.Text = (spec.ufm.RH4r[2] * k).ToString("###0.00");
                txtRH4r_D.Text = (spec.ufm.RH4r[3] * k).ToString("###0.00");

                txtRH5r_A.Text = (spec.ufm.RH5r[0] * k).ToString("###0.00");
                txtRH5r_B.Text = (spec.ufm.RH5r[1] * k).ToString("###0.00");
                txtRH5r_C.Text = (spec.ufm.RH5r[2] * k).ToString("###0.00");
                txtRH5r_D.Text = (spec.ufm.RH5r[3] * k).ToString("###0.00");

                txtRH6r_A.Text = (spec.ufm.RH6r[0] * k).ToString("###0.00");
                txtRH6r_B.Text = (spec.ufm.RH6r[1] * k).ToString("###0.00");
                txtRH6r_C.Text = (spec.ufm.RH6r[2] * k).ToString("###0.00");
                txtRH6r_D.Text = (spec.ufm.RH6r[3] * k).ToString("###0.00");

                txtRH7r_A.Text = (spec.ufm.RH7r[0] * k).ToString("###0.00");
                txtRH7r_B.Text = (spec.ufm.RH7r[1] * k).ToString("###0.00");
                txtRH7r_C.Text = (spec.ufm.RH7r[2] * k).ToString("###0.00");
                txtRH7r_D.Text = (spec.ufm.RH7r[3] * k).ToString("###0.00");

                txtRH8r_A.Text = (spec.ufm.RH8r[0] * k).ToString("###0.00");
                txtRH8r_B.Text = (spec.ufm.RH8r[1] * k).ToString("###0.00");
                txtRH8r_C.Text = (spec.ufm.RH8r[2] * k).ToString("###0.00");
                txtRH8r_D.Text = (spec.ufm.RH8r[3] * k).ToString("###0.00");

                txtRH9r_A.Text = (spec.ufm.RH9r[0] * k).ToString("###0.00");
                txtRH9r_B.Text = (spec.ufm.RH9r[1] * k).ToString("###0.00");
                txtRH9r_C.Text = (spec.ufm.RH9r[2] * k).ToString("###0.00");
                txtRH9r_D.Text = (spec.ufm.RH9r[3] * k).ToString("###0.00");

                txtRH10r_A.Text = (spec.ufm.RH10r[0] * k).ToString("###0.00");
                txtRH10r_B.Text = (spec.ufm.RH10r[1] * k).ToString("###0.00");
                txtRH10r_C.Text = (spec.ufm.RH10r[2] * k).ToString("###0.00");
                txtRH10r_D.Text = (spec.ufm.RH10r[3] * k).ToString("###0.00");

                txtLFVr_A.Text = (spec.ufm.LFVr[0] * k).ToString("###0.00");
                txtLFVr_B.Text = (spec.ufm.LFVr[1] * k).ToString("###0.00");
                txtLFVr_C.Text = (spec.ufm.LFVr[2] * k).ToString("###0.00");
                txtLFVr_D.Text = (spec.ufm.LFVr[3] * k).ToString("###0.00");

                txtLHr_A.Text = (spec.ufm.LHr[0] * k).ToString("###0.00");
                txtLHr_B.Text = (spec.ufm.LHr[1] * k).ToString("###0.00");
                txtLHr_C.Text = (spec.ufm.LHr[2] * k).ToString("###0.00");
                txtLHr_D.Text = (spec.ufm.LHr[3] * k).ToString("###0.00");

                txtLH2r_A.Text = (spec.ufm.LH2r[0] * k).ToString("###0.00");
                txtLH2r_B.Text = (spec.ufm.LH2r[1] * k).ToString("###0.00");
                txtLH2r_C.Text = (spec.ufm.LH2r[2] * k).ToString("###0.00");
                txtLH2r_D.Text = (spec.ufm.LH2r[3] * k).ToString("###0.00");

                txtLH3r_A.Text = (spec.ufm.LH3r[0] * k).ToString("###0.00");
                txtLH3r_B.Text = (spec.ufm.LH3r[1] * k).ToString("###0.00");
                txtLH3r_C.Text = (spec.ufm.LH3r[2] * k).ToString("###0.00");
                txtLH3r_D.Text = (spec.ufm.LH3r[3] * k).ToString("###0.00");

                txtLH4r_A.Text = (spec.ufm.LH4r[0] * k).ToString("###0.00");
                txtLH4r_B.Text = (spec.ufm.LH4r[1] * k).ToString("###0.00");
                txtLH4r_C.Text = (spec.ufm.LH4r[2] * k).ToString("###0.00");
                txtLH4r_D.Text = (spec.ufm.LH4r[3] * k).ToString("###0.00");

                txtLH5r_A.Text = (spec.ufm.LH5r[0] * k).ToString("###0.00");
                txtLH5r_B.Text = (spec.ufm.LH5r[1] * k).ToString("###0.00");
                txtLH5r_C.Text = (spec.ufm.LH5r[2] * k).ToString("###0.00");
                txtLH5r_D.Text = (spec.ufm.LH5r[3] * k).ToString("###0.00");

                txtLH6r_A.Text = (spec.ufm.LH6r[0] * k).ToString("###0.00");
                txtLH6r_B.Text = (spec.ufm.LH6r[1] * k).ToString("###0.00");
                txtLH6r_C.Text = (spec.ufm.LH6r[2] * k).ToString("###0.00");
                txtLH6r_D.Text = (spec.ufm.LH6r[3] * k).ToString("###0.00");

                txtLH7r_A.Text = (spec.ufm.LH7r[0] * k).ToString("###0.00");
                txtLH7r_B.Text = (spec.ufm.LH7r[1] * k).ToString("###0.00");
                txtLH7r_C.Text = (spec.ufm.LH7r[2] * k).ToString("###0.00");
                txtLH7r_D.Text = (spec.ufm.LH7r[3] * k).ToString("###0.00");

                txtLH8r_A.Text = (spec.ufm.LH8r[0] * k).ToString("###0.00");
                txtLH8r_B.Text = (spec.ufm.LH8r[1] * k).ToString("###0.00");
                txtLH8r_C.Text = (spec.ufm.LH8r[2] * k).ToString("###0.00");
                txtLH8r_D.Text = (spec.ufm.LH8r[3] * k).ToString("###0.00");

                txtLH9r_A.Text = (spec.ufm.LH9r[0] * k).ToString("###0.00");
                txtLH9r_B.Text = (spec.ufm.LH9r[1] * k).ToString("###0.00");
                txtLH9r_C.Text = (spec.ufm.LH9r[2] * k).ToString("###0.00");
                txtLH9r_D.Text = (spec.ufm.LH9r[3] * k).ToString("###0.00");

                txtLH10r_A.Text = (spec.ufm.LH10r[0] * k).ToString("###0.00");
                txtLH10r_B.Text = (spec.ufm.LH10r[1] * k).ToString("###0.00");
                txtLH10r_C.Text = (spec.ufm.LH10r[2] * k).ToString("###0.00");
                txtLH10r_D.Text = (spec.ufm.LH10r[3] * k).ToString("###0.00");

                txtCONmax_A.Text = (spec.ufm.CONmax[0] * k).ToString("###0.00");
                txtCONmax_B.Text = (spec.ufm.CONmax[1] * k).ToString("###0.00");
                txtCONmax_C.Text = (spec.ufm.CONmax[2] * k).ToString("###0.00");
                txtCONmax_D.Text = (spec.ufm.CONmax[3] * k).ToString("###0.00");

                txtCONmin_A.Text = (spec.ufm.CONmin[0] * k).ToString("###0.00");
                txtCONmin_B.Text = (spec.ufm.CONmin[1] * k).ToString("###0.00");
                txtCONmin_C.Text = (spec.ufm.CONmin[2] * k).ToString("###0.00");
                txtCONmin_D.Text = (spec.ufm.CONmin[3] * k).ToString("###0.00");

                txtPLYmax_A.Text = (spec.ufm.PLYmax[0] * k).ToString("###0.00");
                txtPLYmax_B.Text = (spec.ufm.PLYmax[1] * k).ToString("###0.00");
                txtPLYmax_C.Text = (spec.ufm.PLYmax[2] * k).ToString("###0.00");
                txtPLYmax_D.Text = (spec.ufm.PLYmax[3] * k).ToString("###0.00");

                txtPLYmin_A.Text = (spec.ufm.PLYmin[0] * k).ToString("###0.00");
                txtPLYmin_B.Text = (spec.ufm.PLYmin[1] * k).ToString("###0.00");
                txtPLYmin_B.Text = (spec.ufm.PLYmin[1] * k).ToString("###0.00");
                txtPLYmin_C.Text = (spec.ufm.PLYmin[2] * k).ToString("###0.00");
                txtPLYmin_D.Text = (spec.ufm.PLYmin[3] * k).ToString("###0.00");

                lblUfRankUnit.Text = s_uUnit.Str;
                lblUfRankUnit1.Text = s_uUnit.Str;
            }
            catch
            {
                return;
            }
        }

        private void TabUfRankGet(Spec spec)
        {
            float k = s_uUnit.Ka;
            spec.ufm.RFV[0] = Convert.ToSingle(txtRFV_A.Text) / k;
            spec.ufm.RFV[1] = Convert.ToSingle(txtRFV_B.Text) / k;
            spec.ufm.RFV[2] = Convert.ToSingle(txtRFV_C.Text) / k;
            spec.ufm.RFV[3] = Convert.ToSingle(txtRFV_D.Text) / k;

            spec.ufm.RH[0] = Convert.ToSingle(txtRH_A.Text) / k;
            spec.ufm.RH[1] = Convert.ToSingle(txtRH_B.Text) / k;
            spec.ufm.RH[2] = Convert.ToSingle(txtRH_C.Text) / k;
            spec.ufm.RH[3] = Convert.ToSingle(txtRH_D.Text) / k;

            spec.ufm.RH2[0] = Convert.ToSingle(txtRH2_A.Text) / k;
            spec.ufm.RH2[1] = Convert.ToSingle(txtRH2_B.Text) / k;
            spec.ufm.RH2[2] = Convert.ToSingle(txtRH2_C.Text) / k;
            spec.ufm.RH2[3] = Convert.ToSingle(txtRH2_D.Text) / k;

            spec.ufm.RH3[0] = Convert.ToSingle(txtRH3_A.Text) / k;
            spec.ufm.RH3[1] = Convert.ToSingle(txtRH3_B.Text) / k;
            spec.ufm.RH3[2] = Convert.ToSingle(txtRH3_C.Text) / k;
            spec.ufm.RH3[3] = Convert.ToSingle(txtRH3_D.Text) / k;

            spec.ufm.RH4[0] = Convert.ToSingle(txtRH4_A.Text) / k;
            spec.ufm.RH4[1] = Convert.ToSingle(txtRH4_B.Text) / k;
            spec.ufm.RH4[2] = Convert.ToSingle(txtRH4_C.Text) / k;
            spec.ufm.RH4[3] = Convert.ToSingle(txtRH4_D.Text) / k;

            spec.ufm.RH5[0] = Convert.ToSingle(txtRH5_A.Text) / k;
            spec.ufm.RH5[1] = Convert.ToSingle(txtRH5_B.Text) / k;
            spec.ufm.RH5[2] = Convert.ToSingle(txtRH5_C.Text) / k;
            spec.ufm.RH5[3] = Convert.ToSingle(txtRH5_D.Text) / k;

            spec.ufm.RH6[0] = Convert.ToSingle(txtRH6_A.Text) / k;
            spec.ufm.RH6[1] = Convert.ToSingle(txtRH6_B.Text) / k;
            spec.ufm.RH6[2] = Convert.ToSingle(txtRH6_C.Text) / k;
            spec.ufm.RH6[3] = Convert.ToSingle(txtRH6_D.Text) / k;

            spec.ufm.RH7[0] = Convert.ToSingle(txtRH7_A.Text) / k;
            spec.ufm.RH7[1] = Convert.ToSingle(txtRH7_B.Text) / k;
            spec.ufm.RH7[2] = Convert.ToSingle(txtRH7_C.Text) / k;
            spec.ufm.RH7[3] = Convert.ToSingle(txtRH7_D.Text) / k;

            spec.ufm.RH8[0] = Convert.ToSingle(txtRH8_A.Text) / k;
            spec.ufm.RH8[1] = Convert.ToSingle(txtRH8_B.Text) / k;
            spec.ufm.RH8[2] = Convert.ToSingle(txtRH8_C.Text) / k;
            spec.ufm.RH8[3] = Convert.ToSingle(txtRH8_D.Text) / k;

            spec.ufm.RH9[0] = Convert.ToSingle(txtRH9_A.Text) / k;
            spec.ufm.RH9[1] = Convert.ToSingle(txtRH9_B.Text) / k;
            spec.ufm.RH9[2] = Convert.ToSingle(txtRH9_C.Text) / k;
            spec.ufm.RH9[3] = Convert.ToSingle(txtRH9_D.Text) / k;

            spec.ufm.RH10[0] = Convert.ToSingle(txtRH10_A.Text) / k;
            spec.ufm.RH10[1] = Convert.ToSingle(txtRH10_B.Text) / k;
            spec.ufm.RH10[2] = Convert.ToSingle(txtRH10_C.Text) / k;
            spec.ufm.RH10[3] = Convert.ToSingle(txtRH10_D.Text) / k;

            spec.ufm.RFVr[0] = Convert.ToSingle(txtRFVr_A.Text) / k;
            spec.ufm.RFVr[1] = Convert.ToSingle(txtRFVr_B.Text) / k;
            spec.ufm.RFVr[2] = Convert.ToSingle(txtRFVr_C.Text) / k;
            spec.ufm.RFVr[3] = Convert.ToSingle(txtRFVr_D.Text) / k;

            spec.ufm.RHr[0] = Convert.ToSingle(txtRHr_A.Text) / k;
            spec.ufm.RHr[1] = Convert.ToSingle(txtRHr_B.Text) / k;
            spec.ufm.RHr[2] = Convert.ToSingle(txtRHr_C.Text) / k;
            spec.ufm.RHr[3] = Convert.ToSingle(txtRHr_D.Text) / k;

            spec.ufm.RH2r[0] = Convert.ToSingle(txtRH2r_A.Text) / k;
            spec.ufm.RH2r[1] = Convert.ToSingle(txtRH2r_B.Text) / k;
            spec.ufm.RH2r[2] = Convert.ToSingle(txtRH2r_C.Text) / k;
            spec.ufm.RH2r[3] = Convert.ToSingle(txtRH2r_D.Text) / k;

            spec.ufm.RH3r[0] = Convert.ToSingle(txtRH3r_A.Text) / k;
            spec.ufm.RH3r[1] = Convert.ToSingle(txtRH3r_B.Text) / k;
            spec.ufm.RH3r[2] = Convert.ToSingle(txtRH3r_C.Text) / k;
            spec.ufm.RH3r[3] = Convert.ToSingle(txtRH3r_D.Text) / k;

            spec.ufm.RH4r[0] = Convert.ToSingle(txtRH4r_A.Text) / k;
            spec.ufm.RH4r[1] = Convert.ToSingle(txtRH4r_B.Text) / k;
            spec.ufm.RH4r[2] = Convert.ToSingle(txtRH4r_C.Text) / k;
            spec.ufm.RH4r[3] = Convert.ToSingle(txtRH4r_D.Text) / k;

            spec.ufm.RH5r[0] = Convert.ToSingle(txtRH5r_A.Text) / k;
            spec.ufm.RH5r[1] = Convert.ToSingle(txtRH5r_B.Text) / k;
            spec.ufm.RH5r[2] = Convert.ToSingle(txtRH5r_C.Text) / k;
            spec.ufm.RH5r[3] = Convert.ToSingle(txtRH5r_D.Text) / k;

            spec.ufm.RH6r[0] = Convert.ToSingle(txtRH6r_A.Text) / k;
            spec.ufm.RH6r[1] = Convert.ToSingle(txtRH6r_B.Text) / k;
            spec.ufm.RH6r[2] = Convert.ToSingle(txtRH6r_C.Text) / k;
            spec.ufm.RH6r[3] = Convert.ToSingle(txtRH6r_D.Text) / k;

            spec.ufm.RH7r[0] = Convert.ToSingle(txtRH7r_A.Text) / k;
            spec.ufm.RH7r[1] = Convert.ToSingle(txtRH7r_B.Text) / k;
            spec.ufm.RH7r[2] = Convert.ToSingle(txtRH7r_C.Text) / k;
            spec.ufm.RH7r[3] = Convert.ToSingle(txtRH7r_D.Text) / k;

            spec.ufm.RH8r[0] = Convert.ToSingle(txtRH8r_A.Text) / k;
            spec.ufm.RH8r[1] = Convert.ToSingle(txtRH8r_B.Text) / k;
            spec.ufm.RH8r[2] = Convert.ToSingle(txtRH8r_C.Text) / k;
            spec.ufm.RH8r[3] = Convert.ToSingle(txtRH8r_D.Text) / k;

            spec.ufm.RH9r[0] = Convert.ToSingle(txtRH9r_A.Text) / k;
            spec.ufm.RH9r[1] = Convert.ToSingle(txtRH9r_B.Text) / k;
            spec.ufm.RH9r[2] = Convert.ToSingle(txtRH9r_C.Text) / k;
            spec.ufm.RH9r[3] = Convert.ToSingle(txtRH9r_D.Text) / k;

            spec.ufm.RH10r[0] = Convert.ToSingle(txtRH10r_A.Text) / k;
            spec.ufm.RH10r[1] = Convert.ToSingle(txtRH10r_B.Text) / k;
            spec.ufm.RH10r[2] = Convert.ToSingle(txtRH10r_C.Text) / k;
            spec.ufm.RH10r[3] = Convert.ToSingle(txtRH10r_D.Text) / k;

            spec.ufm.LFV[0] = Convert.ToSingle(txtLFV_A.Text) / k;
            spec.ufm.LFV[1] = Convert.ToSingle(txtLFV_B.Text) / k;
            spec.ufm.LFV[2] = Convert.ToSingle(txtLFV_C.Text) / k;
            spec.ufm.LFV[3] = Convert.ToSingle(txtLFV_D.Text) / k;

            spec.ufm.LH[0] = Convert.ToSingle(txtLH_A.Text) / k;
            spec.ufm.LH[1] = Convert.ToSingle(txtLH_B.Text) / k;
            spec.ufm.LH[2] = Convert.ToSingle(txtLH_C.Text) / k;
            spec.ufm.LH[3] = Convert.ToSingle(txtLH_D.Text) / k;

            spec.ufm.LH2[0] = Convert.ToSingle(txtLH2_A.Text) / k;
            spec.ufm.LH2[1] = Convert.ToSingle(txtLH2_B.Text) / k;
            spec.ufm.LH2[2] = Convert.ToSingle(txtLH2_C.Text) / k;
            spec.ufm.LH2[3] = Convert.ToSingle(txtLH2_D.Text) / k;

            spec.ufm.LH3[0] = Convert.ToSingle(txtLH3_A.Text) / k;
            spec.ufm.LH3[1] = Convert.ToSingle(txtLH3_B.Text) / k;
            spec.ufm.LH3[2] = Convert.ToSingle(txtLH3_C.Text) / k;
            spec.ufm.LH3[3] = Convert.ToSingle(txtLH3_D.Text) / k;

            spec.ufm.LH4[0] = Convert.ToSingle(txtLH4_A.Text) / k;
            spec.ufm.LH4[1] = Convert.ToSingle(txtLH4_B.Text) / k;
            spec.ufm.LH4[2] = Convert.ToSingle(txtLH4_C.Text) / k;
            spec.ufm.LH4[3] = Convert.ToSingle(txtLH4_D.Text) / k;

            spec.ufm.LH5[0] = Convert.ToSingle(txtLH5_A.Text) / k;
            spec.ufm.LH5[1] = Convert.ToSingle(txtLH5_B.Text) / k;
            spec.ufm.LH5[2] = Convert.ToSingle(txtLH5_C.Text) / k;
            spec.ufm.LH5[3] = Convert.ToSingle(txtLH5_D.Text) / k;

            spec.ufm.LH6[0] = Convert.ToSingle(txtLH6_A.Text) / k;
            spec.ufm.LH6[1] = Convert.ToSingle(txtLH6_B.Text) / k;
            spec.ufm.LH6[2] = Convert.ToSingle(txtLH6_C.Text) / k;
            spec.ufm.LH6[3] = Convert.ToSingle(txtLH6_D.Text) / k;

            spec.ufm.LH7[0] = Convert.ToSingle(txtLH7_A.Text) / k;
            spec.ufm.LH7[1] = Convert.ToSingle(txtLH7_B.Text) / k;
            spec.ufm.LH7[2] = Convert.ToSingle(txtLH7_C.Text) / k;
            spec.ufm.LH7[3] = Convert.ToSingle(txtLH7_D.Text) / k;

            spec.ufm.LH8[0] = Convert.ToSingle(txtLH8_A.Text) / k;
            spec.ufm.LH8[1] = Convert.ToSingle(txtLH8_B.Text) / k;
            spec.ufm.LH8[2] = Convert.ToSingle(txtLH8_C.Text) / k;
            spec.ufm.LH8[3] = Convert.ToSingle(txtLH8_D.Text) / k;

            spec.ufm.LH9[0] = Convert.ToSingle(txtLH9_A.Text) / k;
            spec.ufm.LH9[1] = Convert.ToSingle(txtLH9_B.Text) / k;
            spec.ufm.LH9[2] = Convert.ToSingle(txtLH9_C.Text) / k;
            spec.ufm.LH9[3] = Convert.ToSingle(txtLH9_D.Text) / k;

            spec.ufm.LH10[0] = Convert.ToSingle(txtLH10_A.Text) / k;
            spec.ufm.LH10[1] = Convert.ToSingle(txtLH10_B.Text) / k;
            spec.ufm.LH10[2] = Convert.ToSingle(txtLH10_C.Text) / k;
            spec.ufm.LH10[3] = Convert.ToSingle(txtLH10_D.Text) / k;

            spec.ufm.LFVr[0] = Convert.ToSingle(txtLFVr_A.Text) / k;
            spec.ufm.LFVr[1] = Convert.ToSingle(txtLFVr_B.Text) / k;
            spec.ufm.LFVr[2] = Convert.ToSingle(txtLFVr_C.Text) / k;
            spec.ufm.LFVr[3] = Convert.ToSingle(txtLFVr_D.Text) / k;

            spec.ufm.LHr[0] = Convert.ToSingle(txtLHr_A.Text) / k;
            spec.ufm.LHr[1] = Convert.ToSingle(txtLHr_B.Text) / k;
            spec.ufm.LHr[2] = Convert.ToSingle(txtLHr_C.Text) / k;
            spec.ufm.LHr[3] = Convert.ToSingle(txtLHr_D.Text) / k;

            spec.ufm.LH2r[0] = Convert.ToSingle(txtLH2r_A.Text) / k;
            spec.ufm.LH2r[1] = Convert.ToSingle(txtLH2r_B.Text) / k;
            spec.ufm.LH2r[2] = Convert.ToSingle(txtLH2r_C.Text) / k;
            spec.ufm.LH2r[3] = Convert.ToSingle(txtLH2r_D.Text) / k;

            spec.ufm.LH3r[0] = Convert.ToSingle(txtLH3r_A.Text) / k;
            spec.ufm.LH3r[1] = Convert.ToSingle(txtLH3r_B.Text) / k;
            spec.ufm.LH3r[2] = Convert.ToSingle(txtLH3r_C.Text) / k;
            spec.ufm.LH3r[3] = Convert.ToSingle(txtLH3r_D.Text) / k;

            spec.ufm.LH4r[0] = Convert.ToSingle(txtLH4r_A.Text) / k;
            spec.ufm.LH4r[1] = Convert.ToSingle(txtLH4r_B.Text) / k;
            spec.ufm.LH4r[2] = Convert.ToSingle(txtLH4r_C.Text) / k;
            spec.ufm.LH4r[3] = Convert.ToSingle(txtLH4r_D.Text) / k;

            spec.ufm.LH5r[0] = Convert.ToSingle(txtLH5r_A.Text) / k;
            spec.ufm.LH5r[1] = Convert.ToSingle(txtLH5r_B.Text) / k;
            spec.ufm.LH5r[2] = Convert.ToSingle(txtLH5r_C.Text) / k;
            spec.ufm.LH5r[3] = Convert.ToSingle(txtLH5r_D.Text) / k;

            spec.ufm.LH6r[0] = Convert.ToSingle(txtLH6r_A.Text) / k;
            spec.ufm.LH6r[1] = Convert.ToSingle(txtLH6r_B.Text) / k;
            spec.ufm.LH6r[2] = Convert.ToSingle(txtLH6r_C.Text) / k;
            spec.ufm.LH6r[3] = Convert.ToSingle(txtLH6r_D.Text) / k;

            spec.ufm.LH7r[0] = Convert.ToSingle(txtLH7r_A.Text) / k;
            spec.ufm.LH7r[1] = Convert.ToSingle(txtLH7r_B.Text) / k;
            spec.ufm.LH7r[2] = Convert.ToSingle(txtLH7r_C.Text) / k;
            spec.ufm.LH7r[3] = Convert.ToSingle(txtLH7r_D.Text) / k;

            spec.ufm.LH8r[0] = Convert.ToSingle(txtLH8r_A.Text) / k;
            spec.ufm.LH8r[1] = Convert.ToSingle(txtLH8r_B.Text) / k;
            spec.ufm.LH8r[2] = Convert.ToSingle(txtLH8r_C.Text) / k;
            spec.ufm.LH8r[3] = Convert.ToSingle(txtLH8r_D.Text) / k;

            spec.ufm.LH9r[0] = Convert.ToSingle(txtLH9r_A.Text) / k;
            spec.ufm.LH9r[1] = Convert.ToSingle(txtLH9r_B.Text) / k;
            spec.ufm.LH9r[2] = Convert.ToSingle(txtLH9r_C.Text) / k;
            spec.ufm.LH9r[3] = Convert.ToSingle(txtLH9r_D.Text) / k;

            spec.ufm.LH10r[0] = Convert.ToSingle(txtLH10r_A.Text) / k;
            spec.ufm.LH10r[1] = Convert.ToSingle(txtLH10r_B.Text) / k;
            spec.ufm.LH10r[2] = Convert.ToSingle(txtLH10r_C.Text) / k;
            spec.ufm.LH10r[3] = Convert.ToSingle(txtLH10r_D.Text) / k;

            spec.ufm.CONmax[0] = Convert.ToSingle(txtCONmax_A.Text) / k;
            spec.ufm.CONmax[1] = Convert.ToSingle(txtCONmax_B.Text) / k;
            spec.ufm.CONmax[2] = Convert.ToSingle(txtCONmax_C.Text) / k;
            spec.ufm.CONmax[3] = Convert.ToSingle(txtCONmax_D.Text) / k;

            spec.ufm.CONmin[0] = Convert.ToSingle(txtCONmin_A.Text) / k;
            spec.ufm.CONmin[1] = Convert.ToSingle(txtCONmin_B.Text) / k;
            spec.ufm.CONmin[2] = Convert.ToSingle(txtCONmin_C.Text) / k;
            spec.ufm.CONmin[3] = Convert.ToSingle(txtCONmin_D.Text) / k;

            spec.ufm.PLYmax[0] = Convert.ToSingle(txtPLYmax_A.Text) / k;
            spec.ufm.PLYmax[1] = Convert.ToSingle(txtPLYmax_B.Text) / k;
            spec.ufm.PLYmax[2] = Convert.ToSingle(txtPLYmax_C.Text) / k;
            spec.ufm.PLYmax[3] = Convert.ToSingle(txtPLYmax_D.Text) / k;

            spec.ufm.PLYmin[0] = Convert.ToSingle(txtPLYmin_A.Text) / k;
            spec.ufm.PLYmin[1] = Convert.ToSingle(txtPLYmin_B.Text) / k;
            spec.ufm.PLYmin[2] = Convert.ToSingle(txtPLYmin_C.Text) / k;
            spec.ufm.PLYmin[3] = Convert.ToSingle(txtPLYmin_D.Text) / k;

        }
        //-----uf judge use ---------------------
        private void TabUfJudgSet(Spec spec)
        {
            chkRFV.Checked = spec.ufmJudg.RFV;
            chkRFVr.Checked = spec.ufmJudg.RFVr;
            chkRH.Checked = spec.ufmJudg.RH;
            chkRH2.Checked = spec.ufmJudg.RH2;
            chkRH3.Checked = spec.ufmJudg.RH3;
            chkRH4.Checked = spec.ufmJudg.RH4;
            chkRH5.Checked = spec.ufmJudg.RH5;
            chkRH6.Checked = spec.ufmJudg.RH6;
            chkRH7.Checked = spec.ufmJudg.RH7;
            chkRH8.Checked = spec.ufmJudg.RH8;
            chkRH9.Checked = spec.ufmJudg.RH9;
            chkRH10.Checked = spec.ufmJudg.RH10;
            chkRHr.Checked = spec.ufmJudg.RHr;
            chkRH2r.Checked = spec.ufmJudg.RH2r;
            chkRH3r.Checked = spec.ufmJudg.RH3r;
            chkRH4r.Checked = spec.ufmJudg.RH4r;
            chkRH5r.Checked = spec.ufmJudg.RH5r;
            chkRH6r.Checked = spec.ufmJudg.RH6r;
            chkRH7r.Checked = spec.ufmJudg.RH7r;
            chkRH8r.Checked = spec.ufmJudg.RH8r;
            chkRH9r.Checked = spec.ufmJudg.RH9r;
            chkRH10r.Checked = spec.ufmJudg.RH10r;
            chkLFV.Checked = spec.ufmJudg.LFV;
            chkLH.Checked = spec.ufmJudg.LH;
            chkLH2.Checked = spec.ufmJudg.LH2;
            chkLH3.Checked = spec.ufmJudg.LH3;
            chkLH4.Checked = spec.ufmJudg.LH4;
            chkLH5.Checked = spec.ufmJudg.LH5;
            chkLH6.Checked = spec.ufmJudg.LH6;
            chkLH7.Checked = spec.ufmJudg.LH7;
            chkLH8.Checked = spec.ufmJudg.LH8;
            chkLH9.Checked = spec.ufmJudg.LH9;
            chkLH10.Checked = spec.ufmJudg.LH10;
            chkLFVr.Checked = spec.ufmJudg.LFVr;
            chkLHr.Checked = spec.ufmJudg.LHr;
            chkLH2r.Checked = spec.ufmJudg.LH2r;
            chkLH3r.Checked = spec.ufmJudg.LH3r;
            chkLH4r.Checked = spec.ufmJudg.LH4r;
            chkLH5r.Checked = spec.ufmJudg.LH5r;
            chkLH6r.Checked = spec.ufmJudg.LH6r;
            chkLH7r.Checked = spec.ufmJudg.LH7r;
            chkLH8r.Checked = spec.ufmJudg.LH8r;
            chkLH9r.Checked = spec.ufmJudg.LH9r;
            chkLH10r.Checked = spec.ufmJudg.LH10r;
            chkCON.Checked = spec.ufmJudg.CON;
            chkPLY.Checked = spec.ufmJudg.PLY;
        }

        private void TabUfJudgGet(Spec spec)
        {
            spec.ufmJudg.RFV = chkRFV.Checked;
            spec.ufmJudg.RH = chkRH.Checked;
            spec.ufmJudg.RH2 = chkRH2.Checked;
            spec.ufmJudg.RH3 = chkRH3.Checked;
            spec.ufmJudg.RH4 = chkRH4.Checked;
            spec.ufmJudg.RH5 = chkRH5.Checked;
            spec.ufmJudg.RH6 = chkRH6.Checked;
            spec.ufmJudg.RH7 = chkRH7.Checked;
            spec.ufmJudg.RH8 = chkRH8.Checked;
            spec.ufmJudg.RH9 = chkRH9.Checked;
            spec.ufmJudg.RH10 = chkRH10.Checked;
            spec.ufmJudg.RFVr = chkRFVr.Checked;
            spec.ufmJudg.RHr = chkRHr.Checked;
            spec.ufmJudg.RH2r = chkRH2r.Checked;
            spec.ufmJudg.RH3r = chkRH3r.Checked;
            spec.ufmJudg.RH4r = chkRH4r.Checked;
            spec.ufmJudg.RH5r = chkRH5r.Checked;
            spec.ufmJudg.RH6r = chkRH6r.Checked;
            spec.ufmJudg.RH7r = chkRH7r.Checked;
            spec.ufmJudg.RH8r = chkRH8r.Checked;
            spec.ufmJudg.RH9r = chkRH9r.Checked;
            spec.ufmJudg.RH10r = chkRH10r.Checked;
            spec.ufmJudg.LFV = chkLFV.Checked;
            spec.ufmJudg.LH = chkLH.Checked;
            spec.ufmJudg.LH2 = chkLH2.Checked;
            spec.ufmJudg.LH3 = chkLH3.Checked;
            spec.ufmJudg.LH4 = chkLH4.Checked;
            spec.ufmJudg.LH5 = chkLH5.Checked;
            spec.ufmJudg.LH6 = chkLH6.Checked;
            spec.ufmJudg.LH7 = chkLH7.Checked;
            spec.ufmJudg.LH8 = chkLH8.Checked;
            spec.ufmJudg.LH9 = chkLH9.Checked;
            spec.ufmJudg.LH10 = chkLH10.Checked;
            spec.ufmJudg.LFVr = chkLFVr.Checked;
            spec.ufmJudg.LHr = chkLHr.Checked;
            spec.ufmJudg.LH2r = chkLH2r.Checked;
            spec.ufmJudg.LH3r = chkLH3r.Checked;
            spec.ufmJudg.LH4r = chkLH4r.Checked;
            spec.ufmJudg.LH5r = chkLH5r.Checked;
            spec.ufmJudg.LH6r = chkLH6r.Checked;
            spec.ufmJudg.LH7r = chkLH7r.Checked;
            spec.ufmJudg.LH8r = chkLH8r.Checked;
            spec.ufmJudg.LH9r = chkLH9r.Checked;
            spec.ufmJudg.LH10r = chkLH10r.Checked;
            spec.ufmJudg.CON = chkCON.Checked;
            spec.ufmJudg.PLY = chkPLY.Checked;
        }
        //=====ro rank ==========================-
        private void TabRoRankSet(Spec spec)
        {
            lblRoRankModelNo.Text = (s_ModelNo + 1).ToString();
            txtRT_A.Text = spec.ro.RT[0].ToString("###0.00");
            txtRT_B.Text = spec.ro.RT[1].ToString("###0.00");
            txtRT_C.Text = spec.ro.RT[2].ToString("###0.00");
            txtRT_D.Text = spec.ro.RT[3].ToString("###0.00");

            txtRC_A.Text = spec.ro.RC[0].ToString("###0.00");
            txtRC_B.Text = spec.ro.RC[1].ToString("###0.00");
            txtRC_C.Text = spec.ro.RC[2].ToString("###0.00");
            txtRC_D.Text = spec.ro.RC[3].ToString("###0.00");

            txtRB_A.Text = spec.ro.RB[0].ToString("###0.00");
            txtRB_B.Text = spec.ro.RB[1].ToString("###0.00");
            txtRB_C.Text = spec.ro.RB[2].ToString("###0.00");
            txtRB_D.Text = spec.ro.RB[3].ToString("###0.00");

            txtLT_A.Text = spec.ro.LT[0].ToString("###0.00");
            txtLT_B.Text = spec.ro.LT[1].ToString("###0.00");
            txtLT_C.Text = spec.ro.LT[2].ToString("###0.00");
            txtLT_D.Text = spec.ro.LT[3].ToString("###0.00");

            txtLB_A.Text = spec.ro.LB[0].ToString("###0.00");
            txtLB_B.Text = spec.ro.LB[1].ToString("###0.00");
            txtLB_C.Text = spec.ro.LB[2].ToString("###0.00");
            txtLB_D.Text = spec.ro.LB[3].ToString("###0.00");

            txtRTH_A.Text = spec.ro.RTH[0].ToString("###0.00");
            txtRTH_B.Text = spec.ro.RTH[1].ToString("###0.00");
            txtRTH_C.Text = spec.ro.RTH[2].ToString("###0.00");
            txtRTH_D.Text = spec.ro.RTH[3].ToString("###0.00");

            txtRCH_A.Text = spec.ro.RCH[0].ToString("###0.00");
            txtRCH_B.Text = spec.ro.RCH[1].ToString("###0.00");
            txtRCH_C.Text = spec.ro.RCH[2].ToString("###0.00");
            txtRCH_D.Text = spec.ro.RCH[3].ToString("###0.00");

            txtRBH_A.Text = spec.ro.RBH[0].ToString("###0.00");
            txtRBH_B.Text = spec.ro.RBH[1].ToString("###0.00");
            txtRBH_C.Text = spec.ro.RBH[2].ToString("###0.00");
            txtRBH_D.Text = spec.ro.RBH[3].ToString("###0.00");

            txtLTH_A.Text = spec.ro.LTH[0].ToString("###0.00");
            txtLTH_B.Text = spec.ro.LTH[1].ToString("###0.00");
            txtLTH_C.Text = spec.ro.LTH[2].ToString("###0.00");
            txtLTH_D.Text = spec.ro.LTH[3].ToString("###0.00");

            txtLBH_A.Text = spec.ro.LBH[0].ToString("###0.00");
            txtLBH_B.Text = spec.ro.LBH[1].ToString("###0.00");
            txtLBH_C.Text = spec.ro.LBH[2].ToString("###0.00");
            txtLBH_D.Text = spec.ro.LBH[3].ToString("###0.00");

            txtBulgT_A.Text = spec.ro.BulgT[0].ToString("###0.00");
            txtBulgT_B.Text = spec.ro.BulgT[1].ToString("###0.00");
            txtBulgT_C.Text = spec.ro.BulgT[2].ToString("###0.00");
            txtBulgT_D.Text = spec.ro.BulgT[3].ToString("###0.00");

            txtDentT_A.Text = spec.ro.DentT[0].ToString("###0.00");
            txtDentT_B.Text = spec.ro.DentT[1].ToString("###0.00");
            txtDentT_C.Text = spec.ro.DentT[2].ToString("###0.00");
            txtDentT_D.Text = spec.ro.DentT[3].ToString("###0.00");

            txtBulgB_A.Text = spec.ro.BulgB[0].ToString("###0.00");
            txtBulgB_B.Text = spec.ro.BulgB[1].ToString("###0.00");
            txtBulgB_C.Text = spec.ro.BulgB[2].ToString("###0.00");
            txtBulgB_D.Text = spec.ro.BulgB[3].ToString("###0.00");

            txtDentB_A.Text = spec.ro.DentB[0].ToString("###0.00");
            txtDentB_B.Text = spec.ro.DentB[1].ToString("###0.00");
            txtDentB_C.Text = spec.ro.DentB[2].ToString("###0.00");
            txtDentB_D.Text = spec.ro.DentB[3].ToString("###0.00");

        }

        private void TabRoRankGet(Spec spec)
        {
            spec.ro.RT[0] = Convert.ToSingle(txtRT_A.Text);
            spec.ro.RT[1] = Convert.ToSingle(txtRT_B.Text);
            spec.ro.RT[2] = Convert.ToSingle(txtRT_C.Text);
            spec.ro.RT[3] = Convert.ToSingle(txtRT_D.Text);

            spec.ro.RC[0] = Convert.ToSingle(txtRC_A.Text);
            spec.ro.RC[1] = Convert.ToSingle(txtRC_B.Text);
            spec.ro.RC[2] = Convert.ToSingle(txtRC_C.Text);
            spec.ro.RC[3] = Convert.ToSingle(txtRC_D.Text);

            spec.ro.RB[0] = Convert.ToSingle(txtRB_A.Text);
            spec.ro.RB[1] = Convert.ToSingle(txtRB_B.Text);
            spec.ro.RB[2] = Convert.ToSingle(txtRB_C.Text);
            spec.ro.RB[3] = Convert.ToSingle(txtRB_D.Text);

            spec.ro.LT[0] = Convert.ToSingle(txtLT_A.Text);
            spec.ro.LT[1] = Convert.ToSingle(txtLT_B.Text);
            spec.ro.LT[2] = Convert.ToSingle(txtLT_C.Text);
            spec.ro.LT[3] = Convert.ToSingle(txtLT_D.Text);

            spec.ro.LB[0] = Convert.ToSingle(txtLB_A.Text);
            spec.ro.LB[1] = Convert.ToSingle(txtLB_B.Text);
            spec.ro.LB[2] = Convert.ToSingle(txtLB_C.Text);
            spec.ro.LB[3] = Convert.ToSingle(txtLB_D.Text);

            spec.ro.RTH[0] = Convert.ToSingle(txtRTH_A.Text);
            spec.ro.RTH[1] = Convert.ToSingle(txtRTH_B.Text);
            spec.ro.RTH[2] = Convert.ToSingle(txtRTH_C.Text);
            spec.ro.RTH[3] = Convert.ToSingle(txtRTH_D.Text);

            spec.ro.RCH[0] = Convert.ToSingle(txtRCH_A.Text);
            spec.ro.RCH[1] = Convert.ToSingle(txtRCH_B.Text);
            spec.ro.RCH[2] = Convert.ToSingle(txtRCH_C.Text);
            spec.ro.RCH[3] = Convert.ToSingle(txtRCH_D.Text);

            spec.ro.RBH[0] = Convert.ToSingle(txtRBH_A.Text);
            spec.ro.RBH[1] = Convert.ToSingle(txtRBH_B.Text);
            spec.ro.RBH[2] = Convert.ToSingle(txtRBH_C.Text);
            spec.ro.RBH[3] = Convert.ToSingle(txtRBH_D.Text);

            spec.ro.LTH[0] = Convert.ToSingle(txtLTH_A.Text);
            spec.ro.LTH[1] = Convert.ToSingle(txtLTH_B.Text);
            spec.ro.LTH[2] = Convert.ToSingle(txtLTH_C.Text);
            spec.ro.LTH[3] = Convert.ToSingle(txtLTH_D.Text);

            spec.ro.LBH[0] = Convert.ToSingle(txtLBH_A.Text);
            spec.ro.LBH[1] = Convert.ToSingle(txtLBH_B.Text);
            spec.ro.LBH[2] = Convert.ToSingle(txtLBH_C.Text);
            spec.ro.LBH[3] = Convert.ToSingle(txtLBH_D.Text);

            spec.ro.BulgT[0] = Convert.ToSingle(txtBulgT_A.Text);
            spec.ro.BulgT[1] = Convert.ToSingle(txtBulgT_B.Text);
            spec.ro.BulgT[2] = Convert.ToSingle(txtBulgT_C.Text);
            spec.ro.BulgT[3] = Convert.ToSingle(txtBulgT_D.Text);

            spec.ro.DentT[0] = Convert.ToSingle(txtDentT_A.Text);
            spec.ro.DentT[1] = Convert.ToSingle(txtDentT_B.Text);
            spec.ro.DentT[2] = Convert.ToSingle(txtDentT_C.Text);
            spec.ro.DentT[3] = Convert.ToSingle(txtDentT_D.Text);

            spec.ro.BulgB[0] = Convert.ToSingle(txtBulgB_A.Text);
            spec.ro.BulgB[1] = Convert.ToSingle(txtBulgB_B.Text);
            spec.ro.BulgB[2] = Convert.ToSingle(txtBulgB_C.Text);
            spec.ro.BulgB[3] = Convert.ToSingle(txtBulgB_D.Text);

            spec.ro.DentB[0] = Convert.ToSingle(txtDentB_A.Text);
            spec.ro.DentB[1] = Convert.ToSingle(txtDentB_B.Text);
            spec.ro.DentB[2] = Convert.ToSingle(txtDentB_C.Text);
            spec.ro.DentB[3] = Convert.ToSingle(txtDentB_D.Text);

        }
        //------ ro judge use -----------------------------
        private void TabRoJudgSet(Spec spec)
        {
            chkRT.Checked = spec.roJudg.RT;
            chkRC.Checked = spec.roJudg.RC;
            chkRB.Checked = spec.roJudg.RB;
            chkLT.Checked = spec.roJudg.LT;
            chkLB.Checked = spec.roJudg.LB;
            chkRTH.Checked = spec.roJudg.RTH;
            chkRCH.Checked = spec.roJudg.RCH;
            chkRBH.Checked = spec.roJudg.RBH;
            chkLTH.Checked = spec.roJudg.LTH;
            chkLBH.Checked = spec.roJudg.LBH;
            chkBulgT.Checked = spec.roJudg.BulgT;
            chkDentT.Checked = spec.roJudg.DentT;
            chkBulgB.Checked = spec.roJudg.BulgB;
            chkDentB.Checked = spec.roJudg.DentB;
        }

        private void TabRoJudgGet(Spec spec)
        {
            spec.roJudg.RT = chkRT.Checked;
            spec.roJudg.RC = chkRC.Checked;
            spec.roJudg.RB = chkRB.Checked;
            spec.roJudg.LT = chkLT.Checked;
            spec.roJudg.LB = chkLB.Checked;
            spec.roJudg.RTH = chkRTH.Checked;
            spec.roJudg.RCH = chkRCH.Checked;
            spec.roJudg.RBH = chkRBH.Checked;
            spec.roJudg.LTH = chkLTH.Checked;
            spec.roJudg.LBH = chkLBH.Checked;
            spec.roJudg.BulgT = chkBulgT.Checked;
            spec.roJudg.DentT = chkDentT.Checked;
            spec.roJudg.BulgB = chkBulgB.Checked;
            spec.roJudg.DentB = chkDentB.Checked;
        }
        //-------bal rank --------------------
        private void TabBalRankSet(Spec spec)
        {
           //float kDB = 1F, kMS = 1F, kMC = 1F;
            //String str = "g";
            //string dfmt = "", sfmt = "", cfmt = "";
            /*
                        switch (spec.balCond.DBUnit)
                        {
                            case 1:
                                kDB = MForm.bUnit[0, 1].Ka * spec.size.Bdia / 2;
                                str = MForm.bUnit[0, 1].Str;
                                dfmt = MForm.bUnit[0, 1].Fmt;
                                break;
                            case 3:
                                kDB = MForm.bUnit[1, 0].Ka;
                                str = MForm.bUnit[1, 0].Str;
                                dfmt = MForm.bUnit[1, 0].Fmt;
                                break;
                            case 4:
                                kDB = MForm.bUnit[1, 1].Ka * spec.size.Bdia / 2;
                                str = MForm.bUnit[1, 1].Str;
                                dfmt = MForm.bUnit[1, 1].Fmt;
                                break;
                            case 0:
                            default:
                                kDB = MForm.bUnit[0, 0].Ka;
                                str = MForm.bUnit[0, 0].Str;
                                dfmt = MForm.bUnit[0, 0].Fmt;
                                break;
                        }
                        lblMLRankUnit.Text = str;
                        lblMRRankUnit.Text = str;
                        lblMTRankUnit.Text = str;
            
                        switch (spec.balCond.MSUnit)
                        {
                            case 1:
                                kMS = MForm.bUnit[0, 1].Ka * spec.size.Bdia / 20;
                                str = MForm.bUnit[0, 1].Str;
                                sfmt = MForm.bUnit[0, 1].Fmt;
                                break;
                            case 3:
                                kMS = MForm.bUnit[1, 0].Ka;
                                str = MForm.bUnit[1, 0].Str;
                                sfmt = MForm.bUnit[1, 0].Fmt;
                                break;
                            case 4:
                                kMS = MForm.bUnit[1, 1].Ka * spec.size.Bdia / 20;
                                str = MForm.bUnit[1, 1].Str;
                                sfmt = MForm.bUnit[1, 1].Fmt;
                                break;
                            case 0:
                            default:
                                kMS = MForm.bUnit[0, 0].Ka;
                                str = MForm.bUnit[0, 0].Str;
                                sfmt = MForm.bUnit[0, 0].Fmt;
                                break;
                        }
                        lblMSRankUnit.Text = str;

                        switch (spec.balCond.MCUnit)
                        {
                            case 1:
                                kMC = MForm.bUnit[0, 1].Ka * spec.size.Bdia / 20;
                                str = MForm.bUnit[0, 1].Str;
                                cfmt = MForm.bUnit[0, 1].Fmt;
                                break;
                            case 2:
                                kMC = MForm.bUnit[0, 2].Ka * (spec.size.Bdia / 20) * (spec.size.Haba / 10);
                                str = MForm.bUnit[0, 2].Str;
                                cfmt = MForm.bUnit[0, 2].Fmt;
                                break;
                            case 3:
                                kMC = MForm.bUnit[1, 0].Ka;
                                str = MForm.bUnit[1, 0].Str;
                                cfmt = MForm.bUnit[1, 0].Fmt;
                                break;
                            case 4:
                                kMC = MForm.bUnit[1, 1].Ka * spec.size.Bdia / 20;
                                str = MForm.bUnit[1, 1].Str;
                                cfmt = MForm.bUnit[1, 1].Fmt;
                                break;
                            case 5:
                                kMC = MForm.bUnit[1, 2].Ka * (spec.size.Bdia / 20) * (spec.size.Haba / 10);
                                str = MForm.bUnit[1, 2].Str;
                                cfmt = MForm.bUnit[1, 2].Fmt;
                                break;
                            case 0:
                            default:
                                kMC = MForm.bUnit[0, 0].Ka;
                                str = MForm.bUnit[0, 0].Str;
                                cfmt = MForm.bUnit[0, 0].Fmt;
                                break;
                        }
                        lblMCRankUnit.Text = str;
            */
            s_dbUnit = BalUnitSel(spec.balCond.DBUnit, spec);
            txtML_A.Text = (spec.bal.ML[0] * s_dbUnit.Ka).ToString(s_dbUnit.Fmt);
            txtML_B.Text = (spec.bal.ML[1] * s_dbUnit.Ka).ToString(s_dbUnit.Fmt);
            txtML_C.Text = (spec.bal.ML[2] * s_dbUnit.Ka).ToString(s_dbUnit.Fmt);
            txtML_D.Text = (spec.bal.ML[3] * s_dbUnit.Ka).ToString(s_dbUnit.Fmt);

            txtMR_A.Text = (spec.bal.MR[0] * s_dbUnit.Ka).ToString(s_dbUnit.Fmt);
            txtMR_B.Text = (spec.bal.MR[1] * s_dbUnit.Ka).ToString(s_dbUnit.Fmt);
            txtMR_C.Text = (spec.bal.MR[2] * s_dbUnit.Ka).ToString(s_dbUnit.Fmt);
            txtMR_D.Text = (spec.bal.MR[3] * s_dbUnit.Ka).ToString(s_dbUnit.Fmt);

            txtMT_A.Text = (spec.bal.MT[0] * s_dbUnit.Ka).ToString(s_dbUnit.Fmt);
            txtMT_B.Text = (spec.bal.MT[1] * s_dbUnit.Ka).ToString(s_dbUnit.Fmt);
            txtMT_C.Text = (spec.bal.MT[2] * s_dbUnit.Ka).ToString(s_dbUnit.Fmt);
            txtMT_D.Text = (spec.bal.MT[3] * s_dbUnit.Ka).ToString(s_dbUnit.Fmt);

            lblMLRankUnit.Text = s_dbUnit.Str;
            lblMRRankUnit.Text = s_dbUnit.Str;
            lblMTRankUnit.Text = s_dbUnit.Str;

            s_msUnit = BalUnitSel(spec.balCond.MSUnit, spec);
            txtMS_A.Text = (spec.bal.MS[0] * s_msUnit.Ka).ToString(s_msUnit.Fmt);
            txtMS_B.Text = (spec.bal.MS[1] * s_msUnit.Ka).ToString(s_msUnit.Fmt);
            txtMS_C.Text = (spec.bal.MS[2] * s_msUnit.Ka).ToString(s_msUnit.Fmt);
            txtMS_D.Text = (spec.bal.MS[3] * s_msUnit.Ka).ToString(s_msUnit.Fmt);
            lblMSRankUnit.Text = s_msUnit.Str;

            s_mcUnit = BalUnitSel(spec.balCond.MCUnit, spec);
            txtMC_A.Text = (spec.bal.MC[0] * s_mcUnit.Ka).ToString(s_mcUnit.Fmt);
            txtMC_B.Text = (spec.bal.MC[1] * s_mcUnit.Ka).ToString(s_mcUnit.Fmt);
            txtMC_C.Text = (spec.bal.MC[2] * s_mcUnit.Ka).ToString(s_mcUnit.Fmt);
            txtMC_D.Text = (spec.bal.MC[3] * s_mcUnit.Ka).ToString(s_mcUnit.Fmt);
            lblMCRankUnit.Text = s_mcUnit.Str;

        }

        private void TabBalRankGet(Spec spec)
        {
            Unit unit;
            float kDB = 1F, kMS = 1F, kMC = 1F;


            switch (spec.balCond.DBUnit)
            {
                case 1:
                    kDB = Unit.bUnit[0, 1].Ka * spec.size.Bdia / 2;
                    break;
                case 3:
                    kDB = Unit.bUnit[1, 0].Ka;
                    break;
                case 4:
                    kDB = Unit.bUnit[1, 1].Ka * spec.size.Bdia / 2;
                    break;
                case 0:
                default:
                    kDB = Unit.bUnit[0, 0].Ka;
                    break;
            }
            switch (spec.balCond.MSUnit)
            {
                case 1:
                    kMS = Unit.bUnit[0, 1].Ka * spec.size.Bdia / 20;
                    break;
                case 3:
                    kMS = Unit.bUnit[1, 0].Ka;
                    break;
                case 4:
                    kMS = Unit.bUnit[1, 1].Ka * spec.size.Bdia / 20;
                    break;
                case 0:
                default:
                    kMS = Unit.bUnit[0, 0].Ka;
                    break;
            }
            switch (spec.balCond.MCUnit)
            {
                case 1:
                    kMC = Unit.bUnit[0, 1].Ka * spec.size.Bdia / 20;
                    break;
                case 2:
                    kMC = Unit.bUnit[0, 1].Ka * (spec.size.Bdia / 20) * (spec.size.Haba / 10);
                    break;
                case 3:
                    kMC = Unit.bUnit[1, 0].Ka;
                    break;
                case 4:
                    kMC = Unit.bUnit[1, 1].Ka * spec.size.Bdia / 20;
                    break;
                case 5:
                    kMC = Unit.bUnit[1, 1].Ka * (spec.size.Bdia / 20) * (spec.size.Haba / 10);
                    break;
                case 0:
                default:
                    kMC = Unit.bUnit[0, 0].Ka;
                    break;
            }

            //unit = MForm.BalUnitSel(spec.balCond.DBUnit, spec);

            spec.bal.ML[0] = Convert.ToSingle(txtML_A.Text) / s_dbUnit.Ka;
            spec.bal.ML[1] = Convert.ToSingle(txtML_B.Text) / s_dbUnit.Ka;
            spec.bal.ML[2] = Convert.ToSingle(txtML_C.Text) / s_dbUnit.Ka;
            spec.bal.ML[3] = Convert.ToSingle(txtML_D.Text) / s_dbUnit.Ka;

            spec.bal.MR[0] = Convert.ToSingle(txtMR_A.Text) / s_dbUnit.Ka;
            spec.bal.MR[1] = Convert.ToSingle(txtMR_B.Text) / s_dbUnit.Ka;
            spec.bal.MR[2] = Convert.ToSingle(txtMR_C.Text) / s_dbUnit.Ka;
            spec.bal.MR[3] = Convert.ToSingle(txtMR_D.Text) / s_dbUnit.Ka;

            spec.bal.MT[0] = Convert.ToSingle(txtMT_A.Text) / s_dbUnit.Ka;
            spec.bal.MT[1] = Convert.ToSingle(txtMT_B.Text) / s_dbUnit.Ka;
            spec.bal.MT[2] = Convert.ToSingle(txtMT_C.Text) / s_dbUnit.Ka;
            spec.bal.MT[3] = Convert.ToSingle(txtMT_D.Text) / s_dbUnit.Ka;

            //unit = MForm.BalUnitSel(spec.balCond.DBUnit, spec);
            spec.bal.MS[0] = Convert.ToSingle(txtMS_A.Text) / s_msUnit.Ka;
            spec.bal.MS[1] = Convert.ToSingle(txtMS_B.Text) / s_msUnit.Ka;
            spec.bal.MS[2] = Convert.ToSingle(txtMS_C.Text) / s_msUnit.Ka;
            spec.bal.MS[3] = Convert.ToSingle(txtMS_D.Text) / s_msUnit.Ka;

            //unit = MForm.BalUnitSel(spec.balCond.DBUnit, spec);
            spec.bal.MC[0] = Convert.ToSingle(txtMC_A.Text) / s_mcUnit.Ka;
            spec.bal.MC[1] = Convert.ToSingle(txtMC_B.Text) / s_mcUnit.Ka;
            spec.bal.MC[2] = Convert.ToSingle(txtMC_C.Text) / s_mcUnit.Ka;
            spec.bal.MC[3] = Convert.ToSingle(txtMC_D.Text) / s_mcUnit.Ka;
        }
        //-------bal judge use ---------------
        private void TabBalJudgSet(Spec spec)
        {
            chkML.Checked = spec.balJudg.ML;
            chkMR.Checked = spec.balJudg.MR;
            chkMS.Checked = spec.balJudg.MS;
            chkMC.Checked = spec.balJudg.MC;
            chkMT.Checked = spec.balJudg.MT;
        }

        private void TabBalJudgGet(Spec spec)
        {
            spec.balJudg.ML = chkML.Checked;
            spec.balJudg.MR = chkMR.Checked;
            spec.balJudg.MS = chkMS.Checked;
            spec.balJudg.MC = chkMC.Checked;
            spec.balJudg.MT = chkMT.Checked;
        }

        //-----ufh use ---------------------
        private void TabUfhRankSet(Spec spec)
        {
            try
            {
                float k = s_uUnit.Ka;
                lblUfhRankModelNo.Text = (s_ModelNo + 1).ToString();
                txtHRFV_A.Text = (spec.ufh.RFV[0] * k).ToString("###0.00");
                txtHRFV_B.Text = (spec.ufh.RFV[1] * k).ToString("###0.00");
                txtHRFV_C.Text = (spec.ufh.RFV[2] * k).ToString("###0.00");
                txtHRFV_D.Text = (spec.ufh.RFV[3] * k).ToString("###0.00");

                txtHRH_A.Text = (spec.ufh.RH[0] * k).ToString("###0.00");
                txtHRH_B.Text = (spec.ufh.RH[1] * k).ToString("###0.00");
                txtHRH_C.Text = (spec.ufh.RH[2] * k).ToString("###0.00");
                txtHRH_D.Text = (spec.ufh.RH[3] * k).ToString("###0.00");

                txtHRH2_A.Text = (spec.ufh.RH2[0] * k).ToString("###0.00");
                txtHRH2_B.Text = (spec.ufh.RH2[1] * k).ToString("###0.00");
                txtHRH2_C.Text = (spec.ufh.RH2[2] * k).ToString("###0.00");
                txtHRH2_D.Text = (spec.ufh.RH2[3] * k).ToString("###0.00");

                txtHRH3_A.Text = (spec.ufh.RH3[0] * k).ToString("###0.00");
                txtHRH3_B.Text = (spec.ufh.RH3[1] * k).ToString("###0.00");
                txtHRH3_C.Text = (spec.ufh.RH3[2] * k).ToString("###0.00");
                txtHRH3_D.Text = (spec.ufh.RH3[3] * k).ToString("###0.00");

                txtHRH4_A.Text = (spec.ufh.RH4[0] * k).ToString("###0.00");
                txtHRH4_B.Text = (spec.ufh.RH4[1] * k).ToString("###0.00");
                txtHRH4_C.Text = (spec.ufh.RH4[2] * k).ToString("###0.00");
                txtHRH4_D.Text = (spec.ufh.RH4[3] * k).ToString("###0.00");

                txtHRH5_A.Text = (spec.ufh.RH5[0] * k).ToString("###0.00");
                txtHRH5_B.Text = (spec.ufh.RH5[1] * k).ToString("###0.00");
                txtHRH5_C.Text = (spec.ufh.RH5[2] * k).ToString("###0.00");
                txtHRH5_D.Text = (spec.ufh.RH5[3] * k).ToString("###0.00");

                txtHRH6_A.Text = (spec.ufh.RH6[0] * k).ToString("###0.00");
                txtHRH6_B.Text = (spec.ufh.RH6[1] * k).ToString("###0.00");
                txtHRH6_C.Text = (spec.ufh.RH6[2] * k).ToString("###0.00");
                txtHRH6_D.Text = (spec.ufh.RH6[3] * k).ToString("###0.00");

                txtHRH7_A.Text = (spec.ufh.RH7[0] * k).ToString("###0.00");
                txtHRH7_B.Text = (spec.ufh.RH7[1] * k).ToString("###0.00");
                txtHRH7_C.Text = (spec.ufh.RH7[2] * k).ToString("###0.00");
                txtHRH7_D.Text = (spec.ufh.RH7[3] * k).ToString("###0.00");

                txtHRH8_A.Text = (spec.ufh.RH8[0] * k).ToString("###0.00");
                txtHRH8_B.Text = (spec.ufh.RH8[1] * k).ToString("###0.00");
                txtHRH8_C.Text = (spec.ufh.RH8[2] * k).ToString("###0.00");
                txtHRH8_D.Text = (spec.ufh.RH8[3] * k).ToString("###0.00");

                txtHRH9_A.Text = (spec.ufh.RH9[0] * k).ToString("###0.00");
                txtHRH9_B.Text = (spec.ufh.RH9[1] * k).ToString("###0.00");
                txtHRH9_C.Text = (spec.ufh.RH9[2] * k).ToString("###0.00");
                txtHRH9_D.Text = (spec.ufh.RH9[3] * k).ToString("###0.00");

                txtHRH10_A.Text = (spec.ufh.RH10[0] * k).ToString("###0.00");
                txtHRH10_B.Text = (spec.ufh.RH10[1] * k).ToString("###0.00");
                txtHRH10_C.Text = (spec.ufh.RH10[2] * k).ToString("###0.00");
                txtHRH10_D.Text = (spec.ufh.RH10[3] * k).ToString("###0.00");

                txtHLFV_A.Text = (spec.ufh.LFV[0] * k).ToString("###0.00");
                txtHLFV_B.Text = (spec.ufh.LFV[1] * k).ToString("###0.00");
                txtHLFV_C.Text = (spec.ufh.LFV[2] * k).ToString("###0.00");
                txtHLFV_D.Text = (spec.ufh.LFV[3] * k).ToString("###0.00");

                txtHLH_A.Text = (spec.ufh.LH[0] * k).ToString("###0.00");
                txtHLH_B.Text = (spec.ufh.LH[1] * k).ToString("###0.00");
                txtHLH_C.Text = (spec.ufh.LH[2] * k).ToString("###0.00");
                txtHLH_D.Text = (spec.ufh.LH[3] * k).ToString("###0.00");

                txtHLH2_A.Text = (spec.ufh.LH2[0] * k).ToString("###0.00");
                txtHLH2_B.Text = (spec.ufh.LH2[1] * k).ToString("###0.00");
                txtHLH2_C.Text = (spec.ufh.LH2[2] * k).ToString("###0.00");
                txtHLH2_D.Text = (spec.ufh.LH2[3] * k).ToString("###0.00");

                txtHLH3_A.Text = (spec.ufh.LH3[0] * k).ToString("###0.00");
                txtHLH3_B.Text = (spec.ufh.LH3[1] * k).ToString("###0.00");
                txtHLH3_C.Text = (spec.ufh.LH3[2] * k).ToString("###0.00");
                txtHLH3_D.Text = (spec.ufh.LH3[3] * k).ToString("###0.00");

                txtHLH4_A.Text = (spec.ufh.LH4[0] * k).ToString("###0.00");
                txtHLH4_B.Text = (spec.ufh.LH4[1] * k).ToString("###0.00");
                txtHLH4_C.Text = (spec.ufh.LH4[2] * k).ToString("###0.00");
                txtHLH4_D.Text = (spec.ufh.LH4[3] * k).ToString("###0.00");

                txtHLH5_A.Text = (spec.ufh.LH5[0] * k).ToString("###0.00");
                txtHLH5_B.Text = (spec.ufh.LH5[1] * k).ToString("###0.00");
                txtHLH5_C.Text = (spec.ufh.LH5[2] * k).ToString("###0.00");
                txtHLH5_D.Text = (spec.ufh.LH5[3] * k).ToString("###0.00");

                txtHLH6_A.Text = (spec.ufh.LH6[0] * k).ToString("###0.00");
                txtHLH6_B.Text = (spec.ufh.LH6[1] * k).ToString("###0.00");
                txtHLH6_C.Text = (spec.ufh.LH6[2] * k).ToString("###0.00");
                txtHLH6_D.Text = (spec.ufh.LH6[3] * k).ToString("###0.00");

                txtHLH7_A.Text = (spec.ufh.LH7[0] * k).ToString("###0.00");
                txtHLH7_B.Text = (spec.ufh.LH7[1] * k).ToString("###0.00");
                txtHLH7_C.Text = (spec.ufh.LH7[2] * k).ToString("###0.00");
                txtHLH7_D.Text = (spec.ufh.LH7[3] * k).ToString("###0.00");

                txtHLH8_A.Text = (spec.ufh.LH8[0] * k).ToString("###0.00");
                txtHLH8_B.Text = (spec.ufh.LH8[1] * k).ToString("###0.00");
                txtHLH8_C.Text = (spec.ufh.LH8[2] * k).ToString("###0.00");
                txtHLH8_D.Text = (spec.ufh.LH8[3] * k).ToString("###0.00");

                txtHLH9_A.Text = (spec.ufh.LH9[0] * k).ToString("###0.00");
                txtHLH9_B.Text = (spec.ufh.LH9[1] * k).ToString("###0.00");
                txtHLH9_C.Text = (spec.ufh.LH9[2] * k).ToString("###0.00");
                txtHLH9_D.Text = (spec.ufh.LH9[3] * k).ToString("###0.00");

                txtHLH10_A.Text = (spec.ufh.LH10[0] * k).ToString("###0.00");
                txtHLH10_B.Text = (spec.ufh.LH10[1] * k).ToString("###0.00");
                txtHLH10_C.Text = (spec.ufh.LH10[2] * k).ToString("###0.00");
                txtHLH10_D.Text = (spec.ufh.LH10[3] * k).ToString("###0.00");

                txtHTFV_A.Text = (spec.ufh.TFV[0] * k).ToString("###0.00");
                txtHTFV_B.Text = (spec.ufh.TFV[1] * k).ToString("###0.00");
                txtHTFV_C.Text = (spec.ufh.TFV[2] * k).ToString("###0.00");
                txtHTFV_D.Text = (spec.ufh.TFV[3] * k).ToString("###0.00");

                txtHTH_A.Text = (spec.ufh.TH[0] * k).ToString("###0.00");
                txtHTH_B.Text = (spec.ufh.TH[1] * k).ToString("###0.00");
                txtHTH_C.Text = (spec.ufh.TH[2] * k).ToString("###0.00");
                txtHTH_D.Text = (spec.ufh.TH[3] * k).ToString("###0.00");

                txtHTH2_A.Text = (spec.ufh.TH2[0] * k).ToString("###0.00");
                txtHTH2_B.Text = (spec.ufh.TH2[1] * k).ToString("###0.00");
                txtHTH2_C.Text = (spec.ufh.TH2[2] * k).ToString("###0.00");
                txtHTH2_D.Text = (spec.ufh.TH2[3] * k).ToString("###0.00");

                txtHTH3_A.Text = (spec.ufh.TH3[0] * k).ToString("###0.00");
                txtHTH3_B.Text = (spec.ufh.TH3[1] * k).ToString("###0.00");
                txtHTH3_C.Text = (spec.ufh.TH3[2] * k).ToString("###0.00");
                txtHTH3_D.Text = (spec.ufh.TH3[3] * k).ToString("###0.00");

                txtHTH4_A.Text = (spec.ufh.TH4[0] * k).ToString("###0.00");
                txtHTH4_B.Text = (spec.ufh.TH4[1] * k).ToString("###0.00");
                txtHTH4_C.Text = (spec.ufh.TH4[2] * k).ToString("###0.00");
                txtHTH4_D.Text = (spec.ufh.TH4[3] * k).ToString("###0.00");

                txtHTH5_A.Text = (spec.ufh.TH5[0] * k).ToString("###0.00");
                txtHTH5_B.Text = (spec.ufh.TH5[1] * k).ToString("###0.00");
                txtHTH5_C.Text = (spec.ufh.TH5[2] * k).ToString("###0.00");
                txtHTH5_D.Text = (spec.ufh.TH5[3] * k).ToString("###0.00");

                txtHTH6_A.Text = (spec.ufh.TH6[0] * k).ToString("###0.00");
                txtHTH6_B.Text = (spec.ufh.TH6[1] * k).ToString("###0.00");
                txtHTH6_C.Text = (spec.ufh.TH6[2] * k).ToString("###0.00");
                txtHTH6_D.Text = (spec.ufh.TH6[3] * k).ToString("###0.00");

                txtHTH7_A.Text = (spec.ufh.TH7[0] * k).ToString("###0.00");
                txtHTH7_B.Text = (spec.ufh.TH7[1] * k).ToString("###0.00");
                txtHTH7_C.Text = (spec.ufh.TH7[2] * k).ToString("###0.00");
                txtHTH7_D.Text = (spec.ufh.TH7[3] * k).ToString("###0.00");

                txtHTH8_A.Text = (spec.ufh.TH8[0] * k).ToString("###0.00");
                txtHTH8_B.Text = (spec.ufh.TH8[1] * k).ToString("###0.00");
                txtHTH8_C.Text = (spec.ufh.TH8[2] * k).ToString("###0.00");
                txtHTH8_D.Text = (spec.ufh.TH8[3] * k).ToString("###0.00");

                txtHTH9_A.Text = (spec.ufh.TH9[0] * k).ToString("###0.00");
                txtHTH9_B.Text = (spec.ufh.TH9[1] * k).ToString("###0.00");
                txtHTH9_C.Text = (spec.ufh.TH9[2] * k).ToString("###0.00");
                txtHTH9_D.Text = (spec.ufh.TH9[3] * k).ToString("###0.00");

                txtHTH10_A.Text = (spec.ufh.TH10[0] * k).ToString("###0.00");
                txtHTH10_B.Text = (spec.ufh.TH10[1] * k).ToString("###0.00");
                txtHTH10_C.Text = (spec.ufh.TH10[2] * k).ToString("###0.00");
                txtHTH10_D.Text = (spec.ufh.TH10[3] * k).ToString("###0.00");

                lblUfhRankUnit.Text = s_uUnit.Str;
                lblUfhRankUnit1.Text = s_uUnit.Str;
                lblUfhRankUnit2.Text = s_uUnit.Str;
            }
            catch
            {
                return;
            }
        }

        private void TabUfhRankGet(Spec spec)
        {
            float k = s_uUnit.Ka;
            spec.ufh.RFV[0] = Convert.ToSingle(txtHRFV_A.Text) / k;
            spec.ufh.RFV[1] = Convert.ToSingle(txtHRFV_B.Text) / k;
            spec.ufh.RFV[2] = Convert.ToSingle(txtHRFV_C.Text) / k;
            spec.ufh.RFV[3] = Convert.ToSingle(txtHRFV_D.Text) / k;

            spec.ufh.RH[0] = Convert.ToSingle(txtHRH_A.Text) / k;
            spec.ufh.RH[1] = Convert.ToSingle(txtHRH_B.Text) / k;
            spec.ufh.RH[2] = Convert.ToSingle(txtHRH_C.Text) / k;
            spec.ufh.RH[3] = Convert.ToSingle(txtHRH_D.Text) / k;

            spec.ufh.RH2[0] = Convert.ToSingle(txtHRH2_A.Text) / k;
            spec.ufh.RH2[1] = Convert.ToSingle(txtHRH2_B.Text) / k;
            spec.ufh.RH2[2] = Convert.ToSingle(txtHRH2_C.Text) / k;
            spec.ufh.RH2[3] = Convert.ToSingle(txtHRH2_D.Text) / k;

            spec.ufh.RH3[0] = Convert.ToSingle(txtHRH3_A.Text) / k;
            spec.ufh.RH3[1] = Convert.ToSingle(txtHRH3_B.Text) / k;
            spec.ufh.RH3[2] = Convert.ToSingle(txtHRH3_C.Text) / k;
            spec.ufh.RH3[3] = Convert.ToSingle(txtHRH3_D.Text) / k;

            spec.ufh.RH4[0] = Convert.ToSingle(txtHRH4_A.Text) / k;
            spec.ufh.RH4[1] = Convert.ToSingle(txtHRH4_B.Text) / k;
            spec.ufh.RH4[2] = Convert.ToSingle(txtHRH4_C.Text) / k;
            spec.ufh.RH4[3] = Convert.ToSingle(txtHRH4_D.Text) / k;

            spec.ufh.RH5[0] = Convert.ToSingle(txtHRH5_A.Text) / k;
            spec.ufh.RH5[1] = Convert.ToSingle(txtHRH5_B.Text) / k;
            spec.ufh.RH5[2] = Convert.ToSingle(txtHRH5_C.Text) / k;
            spec.ufh.RH5[3] = Convert.ToSingle(txtHRH5_D.Text) / k;

            spec.ufh.RH6[0] = Convert.ToSingle(txtHRH6_A.Text) / k;
            spec.ufh.RH6[1] = Convert.ToSingle(txtHRH6_B.Text) / k;
            spec.ufh.RH6[2] = Convert.ToSingle(txtHRH6_C.Text) / k;
            spec.ufh.RH6[3] = Convert.ToSingle(txtHRH6_D.Text) / k;

            spec.ufh.RH7[0] = Convert.ToSingle(txtHRH7_A.Text) / k;
            spec.ufh.RH7[1] = Convert.ToSingle(txtHRH7_B.Text) / k;
            spec.ufh.RH7[2] = Convert.ToSingle(txtHRH7_C.Text) / k;
            spec.ufh.RH7[3] = Convert.ToSingle(txtHRH7_D.Text) / k;

            spec.ufh.RH8[0] = Convert.ToSingle(txtHRH8_A.Text) / k;
            spec.ufh.RH8[1] = Convert.ToSingle(txtHRH8_B.Text) / k;
            spec.ufh.RH8[2] = Convert.ToSingle(txtHRH8_C.Text) / k;
            spec.ufh.RH8[3] = Convert.ToSingle(txtHRH8_D.Text) / k;

            spec.ufh.RH9[0] = Convert.ToSingle(txtHRH9_A.Text) / k;
            spec.ufh.RH9[1] = Convert.ToSingle(txtHRH9_B.Text) / k;
            spec.ufh.RH9[2] = Convert.ToSingle(txtHRH9_C.Text) / k;
            spec.ufh.RH9[3] = Convert.ToSingle(txtHRH9_D.Text) / k;

            spec.ufh.RH10[0] = Convert.ToSingle(txtHRH10_A.Text) / k;
            spec.ufh.RH10[1] = Convert.ToSingle(txtHRH10_B.Text) / k;
            spec.ufh.RH10[2] = Convert.ToSingle(txtHRH10_C.Text) / k;
            spec.ufh.RH10[3] = Convert.ToSingle(txtHRH10_D.Text) / k;

            spec.ufh.LFV[0] = Convert.ToSingle(txtHLFV_A.Text) / k;
            spec.ufh.LFV[1] = Convert.ToSingle(txtHLFV_B.Text) / k;
            spec.ufh.LFV[2] = Convert.ToSingle(txtHLFV_C.Text) / k;
            spec.ufh.LFV[3] = Convert.ToSingle(txtHLFV_D.Text) / k;

            spec.ufh.LH[0] = Convert.ToSingle(txtHLH_A.Text) / k;
            spec.ufh.LH[1] = Convert.ToSingle(txtHLH_B.Text) / k;
            spec.ufh.LH[2] = Convert.ToSingle(txtHLH_C.Text) / k;
            spec.ufh.LH[3] = Convert.ToSingle(txtHLH_D.Text) / k;

            spec.ufh.LH2[0] = Convert.ToSingle(txtHLH2_A.Text) / k;
            spec.ufh.LH2[1] = Convert.ToSingle(txtHLH2_B.Text) / k;
            spec.ufh.LH2[2] = Convert.ToSingle(txtHLH2_C.Text) / k;
            spec.ufh.LH2[3] = Convert.ToSingle(txtHLH2_D.Text) / k;

            spec.ufh.LH3[0] = Convert.ToSingle(txtHLH3_A.Text) / k;
            spec.ufh.LH3[1] = Convert.ToSingle(txtHLH3_B.Text) / k;
            spec.ufh.LH3[2] = Convert.ToSingle(txtHLH3_C.Text) / k;
            spec.ufh.LH3[3] = Convert.ToSingle(txtHLH3_D.Text) / k;

            spec.ufh.LH4[0] = Convert.ToSingle(txtHLH4_A.Text) / k;
            spec.ufh.LH4[1] = Convert.ToSingle(txtHLH4_B.Text) / k;
            spec.ufh.LH4[2] = Convert.ToSingle(txtHLH4_C.Text) / k;
            spec.ufh.LH4[3] = Convert.ToSingle(txtHLH4_D.Text) / k;

            spec.ufh.LH5[0] = Convert.ToSingle(txtHLH5_A.Text) / k;
            spec.ufh.LH5[1] = Convert.ToSingle(txtHLH5_B.Text) / k;
            spec.ufh.LH5[2] = Convert.ToSingle(txtHLH5_C.Text) / k;
            spec.ufh.LH5[3] = Convert.ToSingle(txtHLH5_D.Text) / k;

            spec.ufh.LH6[0] = Convert.ToSingle(txtHLH6_A.Text) / k;
            spec.ufh.LH6[1] = Convert.ToSingle(txtHLH6_B.Text) / k;
            spec.ufh.LH6[2] = Convert.ToSingle(txtHLH6_C.Text) / k;
            spec.ufh.LH6[3] = Convert.ToSingle(txtHLH6_D.Text) / k;

            spec.ufh.LH7[0] = Convert.ToSingle(txtHLH7_A.Text) / k;
            spec.ufh.LH7[1] = Convert.ToSingle(txtHLH7_B.Text) / k;
            spec.ufh.LH7[2] = Convert.ToSingle(txtHLH7_C.Text) / k;
            spec.ufh.LH7[3] = Convert.ToSingle(txtHLH7_D.Text) / k;

            spec.ufh.LH8[0] = Convert.ToSingle(txtHLH8_A.Text) / k;
            spec.ufh.LH8[1] = Convert.ToSingle(txtHLH8_B.Text) / k;
            spec.ufh.LH8[2] = Convert.ToSingle(txtHLH8_C.Text) / k;
            spec.ufh.LH8[3] = Convert.ToSingle(txtHLH8_D.Text) / k;

            spec.ufh.LH9[0] = Convert.ToSingle(txtHLH9_A.Text) / k;
            spec.ufh.LH9[1] = Convert.ToSingle(txtHLH9_B.Text) / k;
            spec.ufh.LH9[2] = Convert.ToSingle(txtHLH9_C.Text) / k;
            spec.ufh.LH9[3] = Convert.ToSingle(txtHLH9_D.Text) / k;

            spec.ufh.LH10[0] = Convert.ToSingle(txtHLH10_A.Text) / k;
            spec.ufh.LH10[1] = Convert.ToSingle(txtHLH10_B.Text) / k;
            spec.ufh.LH10[2] = Convert.ToSingle(txtHLH10_C.Text) / k;
            spec.ufh.LH10[3] = Convert.ToSingle(txtHLH10_D.Text) / k;

            spec.ufh.TFV[0] = Convert.ToSingle(txtHTFV_A.Text) / k;
            spec.ufh.TFV[1] = Convert.ToSingle(txtHTFV_B.Text) / k;
            spec.ufh.TFV[2] = Convert.ToSingle(txtHTFV_C.Text) / k;
            spec.ufh.TFV[3] = Convert.ToSingle(txtHTFV_D.Text) / k;

            spec.ufh.TH[0] = Convert.ToSingle(txtHTH_A.Text) / k;
            spec.ufh.TH[1] = Convert.ToSingle(txtHTH_B.Text) / k;
            spec.ufh.TH[2] = Convert.ToSingle(txtHTH_C.Text) / k;
            spec.ufh.TH[3] = Convert.ToSingle(txtHTH_D.Text) / k;

            spec.ufh.TH2[0] = Convert.ToSingle(txtHTH2_A.Text) / k;
            spec.ufh.TH2[1] = Convert.ToSingle(txtHTH2_B.Text) / k;
            spec.ufh.TH2[2] = Convert.ToSingle(txtHTH2_C.Text) / k;
            spec.ufh.TH2[3] = Convert.ToSingle(txtHTH2_D.Text) / k;

            spec.ufh.TH3[0] = Convert.ToSingle(txtHTH3_A.Text) / k;
            spec.ufh.TH3[1] = Convert.ToSingle(txtHTH3_B.Text) / k;
            spec.ufh.TH3[2] = Convert.ToSingle(txtHTH3_C.Text) / k;
            spec.ufh.TH3[3] = Convert.ToSingle(txtHTH3_D.Text) / k;

            spec.ufh.TH4[0] = Convert.ToSingle(txtHTH4_A.Text) / k;
            spec.ufh.TH4[1] = Convert.ToSingle(txtHTH4_B.Text) / k;
            spec.ufh.TH4[2] = Convert.ToSingle(txtHTH4_C.Text) / k;
            spec.ufh.TH4[3] = Convert.ToSingle(txtHTH4_D.Text) / k;

            spec.ufh.TH5[0] = Convert.ToSingle(txtHTH5_A.Text) / k;
            spec.ufh.TH5[1] = Convert.ToSingle(txtHTH5_B.Text) / k;
            spec.ufh.TH5[2] = Convert.ToSingle(txtHTH5_C.Text) / k;
            spec.ufh.TH5[3] = Convert.ToSingle(txtHTH5_D.Text) / k;

            spec.ufh.TH6[0] = Convert.ToSingle(txtHTH6_A.Text) / k;
            spec.ufh.TH6[1] = Convert.ToSingle(txtHTH6_B.Text) / k;
            spec.ufh.TH6[2] = Convert.ToSingle(txtHTH6_C.Text) / k;
            spec.ufh.TH6[3] = Convert.ToSingle(txtHTH6_D.Text) / k;

            spec.ufh.TH7[0] = Convert.ToSingle(txtHTH7_A.Text) / k;
            spec.ufh.TH7[1] = Convert.ToSingle(txtHTH7_B.Text) / k;
            spec.ufh.TH7[2] = Convert.ToSingle(txtHTH7_C.Text) / k;
            spec.ufh.TH7[3] = Convert.ToSingle(txtHTH7_D.Text) / k;

            spec.ufh.TH8[0] = Convert.ToSingle(txtHTH8_A.Text) / k;
            spec.ufh.TH8[1] = Convert.ToSingle(txtHTH8_B.Text) / k;
            spec.ufh.TH8[2] = Convert.ToSingle(txtHTH8_C.Text) / k;
            spec.ufh.TH8[3] = Convert.ToSingle(txtHTH8_D.Text) / k;

            spec.ufh.TH9[0] = Convert.ToSingle(txtHTH9_A.Text) / k;
            spec.ufh.TH9[1] = Convert.ToSingle(txtHTH9_B.Text) / k;
            spec.ufh.TH9[2] = Convert.ToSingle(txtHTH9_C.Text) / k;
            spec.ufh.TH9[3] = Convert.ToSingle(txtHTH9_D.Text) / k;

            spec.ufh.TH10[0] = Convert.ToSingle(txtHTH10_A.Text) / k;
            spec.ufh.TH10[1] = Convert.ToSingle(txtHTH10_B.Text) / k;
            spec.ufh.TH10[2] = Convert.ToSingle(txtHTH10_C.Text) / k;
            spec.ufh.TH10[3] = Convert.ToSingle(txtHTH10_D.Text) / k;

        }
        private void TabUfhJudgSet(Spec spec)
        {
            chkHRFV.Checked = spec.ufhJudg.RFV;
            chkHRH.Checked = spec.ufhJudg.RH;
            chkHRH2.Checked = spec.ufhJudg.RH2;
            chkHRH3.Checked = spec.ufhJudg.RH3;
            chkHRH4.Checked = spec.ufhJudg.RH4;
            chkHRH5.Checked = spec.ufhJudg.RH5;
            chkHRH6.Checked = spec.ufhJudg.RH6;
            chkHRH7.Checked = spec.ufhJudg.RH7;
            chkHRH8.Checked = spec.ufhJudg.RH8;
            chkHRH9.Checked = spec.ufhJudg.RH9;
            chkHRH10.Checked = spec.ufhJudg.RH10;

            chkHLFV.Checked = spec.ufhJudg.LFV;
            chkHLH.Checked = spec.ufhJudg.LH;
            chkHLH2.Checked = spec.ufhJudg.LH2;
            chkHLH3.Checked = spec.ufhJudg.LH3;
            chkHLH4.Checked = spec.ufhJudg.LH4;
            chkHLH5.Checked = spec.ufhJudg.LH5;
            chkHLH6.Checked = spec.ufhJudg.LH6;
            chkHLH7.Checked = spec.ufhJudg.LH7;
            chkHLH8.Checked = spec.ufhJudg.LH8;
            chkHLH9.Checked = spec.ufhJudg.LH9;
            chkHLH10.Checked = spec.ufhJudg.LH10;

            chkHTFV.Checked = spec.ufhJudg.TFV;
            chkHTH.Checked = spec.ufhJudg.TH;
            chkHTH2.Checked = spec.ufhJudg.TH2;
            chkHTH3.Checked = spec.ufhJudg.TH3;
            chkHTH4.Checked = spec.ufhJudg.TH4;
            chkHTH5.Checked = spec.ufhJudg.TH5;
            chkHTH6.Checked = spec.ufhJudg.TH6;
            chkHTH7.Checked = spec.ufhJudg.TH7;
            chkHTH8.Checked = spec.ufhJudg.TH8;
            chkHTH9.Checked = spec.ufhJudg.TH9;
            chkHTH10.Checked = spec.ufhJudg.TH10;
        }

        private void TabUfhJudgGet(Spec spec)
        {
            spec.ufhJudg.RFV = chkHRFV.Checked;
            spec.ufhJudg.RH = chkHRH.Checked;
            spec.ufhJudg.RH2 = chkHRH2.Checked;
            spec.ufhJudg.RH3 = chkHRH3.Checked;
            spec.ufhJudg.RH4 = chkHRH4.Checked;
            spec.ufhJudg.RH5 = chkHRH5.Checked;
            spec.ufhJudg.RH6 = chkHRH6.Checked;
            spec.ufhJudg.RH7 = chkHRH7.Checked;
            spec.ufhJudg.RH8 = chkHRH8.Checked;
            spec.ufhJudg.RH9 = chkHRH9.Checked;
            spec.ufhJudg.RH10 = chkHRH10.Checked;

            spec.ufhJudg.LFV = chkHLFV.Checked;
            spec.ufhJudg.LH = chkHLH.Checked;
            spec.ufhJudg.LH2 = chkHLH2.Checked;
            spec.ufhJudg.LH3 = chkHLH3.Checked;
            spec.ufhJudg.LH4 = chkHLH4.Checked;
            spec.ufhJudg.LH5 = chkHLH5.Checked;
            spec.ufhJudg.LH6 = chkHLH6.Checked;
            spec.ufhJudg.LH7 = chkHLH7.Checked;
            spec.ufhJudg.LH8 = chkHLH8.Checked;
            spec.ufhJudg.LH9 = chkHLH9.Checked;
            spec.ufhJudg.LH10 = chkHLH10.Checked;

            spec.ufhJudg.TFV = chkHTFV.Checked;
            spec.ufhJudg.TH = chkHTH.Checked;
            spec.ufhJudg.TH2 = chkHTH2.Checked;
            spec.ufhJudg.TH3 = chkHTH3.Checked;
            spec.ufhJudg.TH4 = chkHTH4.Checked;
            spec.ufhJudg.TH5 = chkHTH5.Checked;
            spec.ufhJudg.TH6 = chkHTH6.Checked;
            spec.ufhJudg.TH7 = chkHTH7.Checked;
            spec.ufhJudg.TH8 = chkHTH8.Checked;
            spec.ufhJudg.TH9 = chkHTH9.Checked;
            spec.ufhJudg.TH10 = chkHTH10.Checked;

        }

        //=========== measure =====================
        private void TabTestUseSet(Spec spec)
        {
            chkUfUse.Enabled = (Sys.UfmUse == 1);   //use unuse の設定
            chkConUse.Enabled = (Sys.UfmUse == 1);
            chkRoUse.Enabled = (Sys.RoUse == 1);
            chkBulgUse.Enabled = (Sys.RoUse == 1);
            chkBalUse.Enabled = (Sys.BalUse == 1);
            chkUfhUse.Enabled = (Sys.UfhUse == 1);

            lblMeasModelNo.Text = (s_ModelNo + 1).ToString();
            chkUfUse.Checked = spec.ufCond.Use;
            chkConUse.Checked = spec.ufCond.ConUse;
            chkRoUse.Checked = spec.roCond.Use;
            chkBulgUse.Checked = spec.roCond.BulgUse;
            chkBalUse.Checked = spec.balCond.Use;
            chkUfhUse.Checked = spec.ufhCond.Use;
            chkPassTire.Checked = spec.ufCond.Pass;

        }

        private void TabTestUseGet(Spec spec)
        {
            spec.ufCond.Use = chkUfUse.Checked;
            spec.ufCond.ConUse = chkConUse.Checked;
            spec.roCond.Use = chkRoUse.Checked;
            spec.roCond.BulgUse = chkBulgUse.Checked;
            spec.balCond.Use = chkBalUse.Checked;
            spec.ufhCond.Use = chkUfhUse.Checked;
            spec.ufCond.Pass = chkPassTire.Checked;

        }

        private void TabUfCondSet(Spec spec)
        {
            float k = s_uUnit.Ka;
            txtLoAir.Text = spec.ufCond.LoAir.ToString("##0.0000"); //airは設定単位で保持変換しない
            txtHiAir.Text = spec.ufCond.HiAir.ToString("##0.0000");
            //txtUfUnit.Text = spec.ufCond.Unit.ToString("###0");
            txtLoad.Text = (spec.ufCond.Load * k).ToString("###0.00");
            txtLoadRange.Text = (spec.ufCond.LoadRange * k).ToString("###0.00");
            txtUfRpm.Text = spec.ufCond.Rpm.ToString("###0.00");
            txtUfRpmKa.Text = spec.ufCond.RpmKa.ToString("###0.00");
            txtCwWarmUp.Text = spec.ufCond.CwWarmUp.ToString("###0.0");
            txtCcwWarmUp.Text = spec.ufCond.CcwWarmUp.ToString("###0.0");
            txtUfSmpAveNum.Text = spec.ufCond.SmpAveNum.ToString("###0");
            txtTireSpring.Text = (spec.ufCond.TireSpring * k).ToString("###0.00");
            txtTireSpringRange.Text = (spec.ufCond.TireSpringRange * k).ToString("###0.00");
            txtUfToolNo.Text = (spec.ufCond.ToolNo + 1).ToString("###0");
            txtUfWaveUse.Text = spec.ufCond.WaveUse.ToString("###0");

            lblLoad.Text = s_uUnit.Str;
            lblLoadRange.Text = s_uUnit.Str;
            lblTSpring.Text = s_uUnit.Str + "/mm";
            lblTSpringRange.Text = s_uUnit.Str + "/mm";
            switch (Sys.AirUnit)            //2010.07.12  空気圧の単位
            {
                case 0:
                    lblLoAirUnit.Text = "MPa";
                    lblHiAirUnit.Text = "MPa";
                    break;
                case 1:
                    lblLoAirUnit.Text = "KPa";
                    lblHiAirUnit.Text = "KPa";
                    break;
                case 2:
                    lblLoAirUnit.Text = "psi";
                    lblHiAirUnit.Text = "psi";
                    break;
                case 3:
                    lblLoAirUnit.Text = "kgf/cm2";
                    lblHiAirUnit.Text = "kgf/cm2";
                    break;
            }                
        }

        private void TabUfCondGet(Spec spec)
        {
            float k = s_uUnit.Ka;
            spec.ufCond.LoAir = Convert.ToSingle(txtLoAir.Text);
            spec.ufCond.HiAir = Convert.ToSingle(txtHiAir.Text);
            //spec.ufCond.Unit = Convert.ToInt32(txtUfUnit.Text);
            spec.ufCond.Load = Convert.ToSingle(txtLoad.Text) / k;
            spec.ufCond.LoadRange = Convert.ToSingle(txtLoadRange.Text) / k;
            spec.ufCond.Rpm = Convert.ToSingle(txtUfRpm.Text);
            spec.ufCond.RpmKa = Convert.ToSingle(txtUfRpmKa.Text);
            spec.ufCond.CwWarmUp = Convert.ToSingle(txtCwWarmUp.Text);
            spec.ufCond.CcwWarmUp = Convert.ToSingle(txtCcwWarmUp.Text);
            spec.ufCond.SmpAveNum = Convert.ToSingle(txtUfSmpAveNum.Text);
            spec.ufCond.TireSpring = Convert.ToSingle(txtTireSpring.Text) / k;
            spec.ufCond.TireSpringRange = Convert.ToSingle(txtTireSpringRange.Text) / k;
            spec.ufCond.ToolNo = Convert.ToInt32(txtUfToolNo.Text) - 1;
            spec.ufCond.WaveUse = Convert.ToInt32(txtUfWaveUse.Text);

        }

        private void TabUfhCondSet(Spec spec)
        {
            float k = s_uUnit.Ka;
            cmbHiDir.SelectedIndex = spec.ufhCond.Dir;
            txtHiSpeed.Text = spec.ufhCond.Speed.ToString("###0.00");
            txtHiWarmUp.Text = spec.ufhCond.WarmUp.ToString("###0.00");
            txtHiSmpAveNum.Text = spec.ufhCond.SmpAveNum.ToString("###0");
            txtFreeUse.Text = spec.ufhCond.FreeUse.ToString("###0");
            txtHiToolNo.Text = (spec.ufhCond.ToolNo + 1).ToString("###0");
            txtHiWaveUse.Text = spec.ufhCond.WaveUse.ToString("###0");

        }

        private void TabUfhCondGet(Spec spec)
        {
            float k = s_uUnit.Ka;
            spec.ufhCond.Dir = cmbHiDir.SelectedIndex;
            spec.ufhCond.Speed = Convert.ToSingle(txtHiSpeed.Text);
            spec.ufhCond.WarmUp = Convert.ToSingle(txtHiWarmUp.Text);
            spec.ufhCond.SmpAveNum = Convert.ToInt32(txtHiSmpAveNum.Text);
            spec.ufhCond.FreeUse = Convert.ToInt32(txtFreeUse.Text);
            spec.ufhCond.ToolNo = Convert.ToInt32(txtHiToolNo.Text) - 1;
            spec.ufhCond.WaveUse = Convert.ToInt32(txtHiWaveUse.Text);
        }

        private void TabUfMarkSet(Spec spec)
        {
            lblMarkModelNo.Text = (s_ModelNo + 1).ToString();
            chkUfMkPinA1.Checked = ((spec.ufCond.MarkPinA & 0x1) == 0x1);
            chkUfMkPinA2.Checked = ((spec.ufCond.MarkPinA & 0x2) == 0x2);
            chkUfMkPinA3.Checked = ((spec.ufCond.MarkPinA & 0x4) == 0x4);
            chkUfMkPinA4.Checked = ((spec.ufCond.MarkPinA & 0x8) == 0x8);
            chkUfMkPinA5.Checked = ((spec.ufCond.MarkPinA & 0x10) == 0x10);

            chkUfMkPinB1.Checked = ((spec.ufCond.MarkPinB & 0x1) == 0x1);
            chkUfMkPinB2.Checked = ((spec.ufCond.MarkPinB & 0x2) == 0x2);
            chkUfMkPinB3.Checked = ((spec.ufCond.MarkPinB & 0x4) == 0x4);
            chkUfMkPinB4.Checked = ((spec.ufCond.MarkPinB & 0x8) == 0x8);
            chkUfMkPinB5.Checked = ((spec.ufCond.MarkPinB & 0x10) == 0x10);

            chkUfMkPinC1.Checked = ((spec.ufCond.MarkPinC & 0x1) == 0x1);
            chkUfMkPinC2.Checked = ((spec.ufCond.MarkPinC & 0x2) == 0x2);
            chkUfMkPinC3.Checked = ((spec.ufCond.MarkPinC & 0x4) == 0x4);
            chkUfMkPinC4.Checked = ((spec.ufCond.MarkPinC & 0x8) == 0x8);
            chkUfMkPinC5.Checked = ((spec.ufCond.MarkPinC & 0x10) == 0x10);

            chkUfMkPinD1.Checked = ((spec.ufCond.MarkPinD & 0x1) == 0x1);
            chkUfMkPinD2.Checked = ((spec.ufCond.MarkPinD & 0x2) == 0x2);
            chkUfMkPinD3.Checked = ((spec.ufCond.MarkPinD & 0x4) == 0x4);
            chkUfMkPinD4.Checked = ((spec.ufCond.MarkPinD & 0x8) == 0x8);
            chkUfMkPinD5.Checked = ((spec.ufCond.MarkPinD & 0x10) == 0x10);

            chkRoMkPinBPS1.Checked = ((spec.roCond.MarkPinBPS & 0x1) == 0x1);
            chkRoMkPinBPS2.Checked = ((spec.roCond.MarkPinBPS & 0x2) == 0x2);
            chkRoMkPinBPS3.Checked = ((spec.roCond.MarkPinBPS & 0x4) == 0x4);
            chkRoMkPinBPS4.Checked = ((spec.roCond.MarkPinBPS & 0x6) == 0x8);
            chkRoMkPinBPS5.Checked = ((spec.roCond.MarkPinBPS & 0x10) == 0x10);

            radRfvOA.Checked = ((spec.ufCond.MarkSel) == 0x0);
            radRfv1H.Checked = ((spec.ufCond.MarkSel) == 0x1);
            radLfvOA.Checked = ((spec.ufCond.MarkSel) == 0x2);
            radLfv1H.Checked = ((spec.ufCond.MarkSel) == 0x3);

            chkUfMarkUse.Checked = ((spec.ufCond.MarkUse & 0x1) == 0x1);

            if (Sys.UfMarkHiUse == 0)
                spec.ufCond.MarkHL = 1; //UF マークを強制的にHiにする
            radUfMkHi.Checked = ((spec.ufCond.MarkHL & 0x1) == 0x1);
            radUfMkLo.Checked = ((spec.ufCond.MarkHL & 0x1) == 0x0);

            radMarkCon.Checked = ((spec.ufCond.MarkCon) == 0x0);
            radMarkConOps.Checked = ((spec.ufCond.MarkCon) == 0x1);
            radMarkUp.Checked = ((spec.ufCond.MarkCon) == 0x2);
            radMarkLo.Checked = ((spec.ufCond.MarkCon) == 0x3);

            chkMkOpt0.Checked = ((spec.ufCond.MarkOpt & 0x1) == 0x1);
            chkMkOpt1.Checked = ((spec.ufCond.MarkOpt & 0x2) == 0x2);
            chkMkOpt2.Checked = ((spec.ufCond.MarkOpt & 0x4) == 0x4);
            chkMkOpt3.Checked = ((spec.ufCond.MarkOpt & 0x8) == 0x8);
            chkMkOpt4.Checked = ((spec.ufCond.MarkOpt & 0x10) == 0x10);
            chkMkOpt5.Checked = ((spec.ufCond.MarkOpt & 0x20) == 0x20);
            chkMkOpt6.Checked = ((spec.ufCond.MarkOpt & 0x40) == 0x40);
            chkMkOpt7.Checked = ((spec.ufCond.MarkOpt & 0x80) == 0x80);

        }

        private void TabUfMarkGet(Spec spec)
        {
            spec.ufCond.MarkPinA = 0;
            if (chkUfMkPinA1.Checked == true)
                spec.ufCond.MarkPinA = 1;
            if (chkUfMkPinA2.Checked == true)
                spec.ufCond.MarkPinA += 2;
            if (chkUfMkPinA3.Checked == true)
                spec.ufCond.MarkPinA += 4;
            if (chkUfMkPinA4.Checked == true)
                spec.ufCond.MarkPinA += 8;
            if (chkUfMkPinA5.Checked == true)
                spec.ufCond.MarkPinA += 0x10;

            spec.ufCond.MarkPinB = 0;
            if (chkUfMkPinB1.Checked == true)
                spec.ufCond.MarkPinB = 1;
            if (chkUfMkPinB2.Checked == true)
                spec.ufCond.MarkPinB += 2;
            if (chkUfMkPinB3.Checked == true)
                spec.ufCond.MarkPinB += 4;
            if (chkUfMkPinB4.Checked == true)
                spec.ufCond.MarkPinB += 8;
            if (chkUfMkPinB5.Checked == true)
                spec.ufCond.MarkPinB += 0x10;

            spec.ufCond.MarkPinC = 0;
            if (chkUfMkPinC1.Checked == true)
                spec.ufCond.MarkPinC = 1;
            if (chkUfMkPinC2.Checked == true)
                spec.ufCond.MarkPinC += 2;
            if (chkUfMkPinC3.Checked == true)
                spec.ufCond.MarkPinC += 4;
            if (chkUfMkPinC4.Checked == true)
                spec.ufCond.MarkPinC += 8;
            if (chkUfMkPinC5.Checked == true)
                spec.ufCond.MarkPinC += 0x10;

            spec.ufCond.MarkPinD = 0;
            if (chkUfMkPinD1.Checked == true)
                spec.ufCond.MarkPinD = 1;
            if (chkUfMkPinD2.Checked == true)
                spec.ufCond.MarkPinD += 2;
            if (chkUfMkPinD3.Checked == true)
                spec.ufCond.MarkPinD += 4;
            if (chkUfMkPinD4.Checked == true)
                spec.ufCond.MarkPinD += 8;
            if (chkUfMkPinD5.Checked == true)
                spec.ufCond.MarkPinD += 0x10;

            spec.roCond.MarkPinBPS = 0;
            if (chkRoMkPinBPS1.Checked == true)
                spec.roCond.MarkPinBPS = 1;
            if (chkRoMkPinBPS2.Checked == true)
                spec.roCond.MarkPinBPS += 2;
            if (chkRoMkPinBPS3.Checked == true)
                spec.roCond.MarkPinBPS += 4;
            if (chkRoMkPinBPS4.Checked == true)
                spec.roCond.MarkPinBPS += 8;
            if (chkRoMkPinBPS5.Checked == true)
                spec.roCond.MarkPinBPS += 0x10;

            spec.ufCond.MarkSel = 0;
            if (radRfvOA.Checked == true)
                spec.ufCond.MarkSel = 0;
            if (radRfv1H.Checked == true)
                spec.ufCond.MarkSel = 1;
            if (radLfvOA.Checked == true)
                spec.ufCond.MarkSel = 2;
            if (radLfv1H.Checked == true)
                spec.ufCond.MarkSel = 3;

            if (chkUfMarkUse.Checked == true)
                spec.ufCond.MarkUse = 1;
            else
                spec.ufCond.MarkUse = 0;

            if (radUfMkHi.Checked == true)
                spec.ufCond.MarkHL = 1;
            else
                spec.ufCond.MarkHL = 0;

            spec.ufCond.MarkCon = 0;
            if (radMarkCon.Checked == true)
                spec.ufCond.MarkCon = 0;
            if (radMarkConOps.Checked == true)
                spec.ufCond.MarkCon = 1;
            if (radMarkUp.Checked == true)
                spec.ufCond.MarkCon = 2;
            if (radMarkLo.Checked == true)
                spec.ufCond.MarkCon = 3;

            spec.ufCond.MarkOpt = 0;
            if (chkMkOpt0.Checked == true)
                spec.ufCond.MarkOpt = 1;
            if (chkMkOpt1.Checked == true)
                spec.ufCond.MarkOpt += 2;
            if (chkMkOpt2.Checked == true)
                spec.ufCond.MarkOpt += 4;
            if (chkMkOpt3.Checked == true)
                spec.ufCond.MarkOpt += 8;
            if (chkMkOpt4.Checked == true)
                spec.ufCond.MarkOpt += 0x10;
            if (chkMkOpt5.Checked == true)
                spec.ufCond.MarkOpt += 0x20;
            if (chkMkOpt6.Checked == true)
                spec.ufCond.MarkOpt += 0x40;
            if (chkMkOpt7.Checked == true)
                spec.ufCond.MarkOpt += 0x80;
        }

        private void TabBalMarkSet(Spec spec)
        {
            chkBalMkPinA1.Checked = ((spec.balCond.MarkPinA & 0x1) == 0x1);
            chkBalMkPinA2.Checked = ((spec.balCond.MarkPinA & 0x2) == 0x2);
            chkBalMkPinA3.Checked = ((spec.balCond.MarkPinA & 0x4) == 0x4);

            chkBalMkPinB1.Checked = ((spec.balCond.MarkPinB & 0x1) == 0x1);
            chkBalMkPinB2.Checked = ((spec.balCond.MarkPinB & 0x2) == 0x2);
            chkBalMkPinB3.Checked = ((spec.balCond.MarkPinB & 0x4) == 0x4);

            chkBalMkPinC1.Checked = ((spec.balCond.MarkPinC & 0x1) == 0x1);
            chkBalMkPinC2.Checked = ((spec.balCond.MarkPinC & 0x2) == 0x2);
            chkBalMkPinC3.Checked = ((spec.balCond.MarkPinC & 0x4) == 0x4);

            chkBalMkPinD1.Checked = ((spec.balCond.MarkPinD & 0x1) == 0x1);
            chkBalMkPinD2.Checked = ((spec.balCond.MarkPinD & 0x2) == 0x2);
            chkBalMkPinD3.Checked = ((spec.balCond.MarkPinD & 0x4) == 0x4);

            radBalML.Checked = ((spec.balCond.MarkSel) == 0x0);
            radBalMR.Checked = ((spec.balCond.MarkSel) == 0x1);
            radBalMS.Checked = ((spec.balCond.MarkSel) == 0x2);
            radBalMC.Checked = ((spec.balCond.MarkSel) == 0x3);

            chkBalMkUse.Checked = ((spec.balCond.MarkUse) == true);

            if (Sys.DbMarkHiUse == 0)
                spec.balCond.MarkHL = 0;    //強制的に軽点マークにする。
            radBalMkHi.Checked = ((spec.balCond.MarkHL & 0x1) == 0x1);
            radBalMkLo.Checked = ((spec.balCond.MarkHL & 0x1) == 0x0);
        }

        private void TabBalMarkGet(Spec spec)
        {
            spec.balCond.MarkPinA = 0;
            if (chkBalMkPinA1.Checked == true)
                spec.balCond.MarkPinA = 1;
            if (chkBalMkPinA2.Checked == true)
                spec.balCond.MarkPinA += 2;
            if (chkBalMkPinA3.Checked == true)
                spec.balCond.MarkPinA += 4;

            spec.balCond.MarkPinB = 0;
            if (chkBalMkPinB1.Checked == true)
                spec.balCond.MarkPinB = 1;
            if (chkBalMkPinB2.Checked == true)
                spec.balCond.MarkPinB += 2;
            if (chkBalMkPinB3.Checked == true)
                spec.balCond.MarkPinB += 4;

            spec.balCond.MarkPinC = 0;
            if (chkBalMkPinC1.Checked == true)
                spec.balCond.MarkPinC = 1;
            if (chkBalMkPinC2.Checked == true)
                spec.balCond.MarkPinC += 2;
            if (chkBalMkPinC3.Checked == true)
                spec.balCond.MarkPinC += 4;

            spec.balCond.MarkPinD = 0;
            if (chkBalMkPinD1.Checked == true)
                spec.balCond.MarkPinD = 1;
            if (chkBalMkPinD2.Checked == true)
                spec.balCond.MarkPinD += 2;
            if (chkBalMkPinD3.Checked == true)
                spec.balCond.MarkPinD += 4;

            spec.balCond.MarkSel = 0;
            if (radBalML.Checked == true)
                spec.balCond.MarkSel = 0;
            if (radBalMR.Checked == true)
                spec.balCond.MarkSel = 1;
            if (radBalMS.Checked == true)
                spec.balCond.MarkSel = 2;
            if (radBalMC.Checked == true)
                spec.balCond.MarkSel = 3;

            if (chkBalMkUse.Checked == true)
                spec.balCond.MarkUse = true;
            else
                spec.balCond.MarkUse = false;

            if (radBalMkHi.Checked == true)
                spec.balCond.MarkHL = 1;
            else
                spec.balCond.MarkHL = 0;


        }

        private void TabSorterSet(Spec spec)
        {
            radSortA1.Checked = ((spec.ufCond.SortA) == 0x0);
            radSortA2.Checked = ((spec.ufCond.SortA) == 0x1);
            radSortA3.Checked = ((spec.ufCond.SortA) == 0x2);

            radSortB1.Checked = ((spec.ufCond.SortB) == 0x0);
            radSortB2.Checked = ((spec.ufCond.SortB) == 0x1);
            radSortB3.Checked = ((spec.ufCond.SortB) == 0x2);

            radSortC1.Checked = ((spec.ufCond.SortC) == 0x0);
            radSortC2.Checked = ((spec.ufCond.SortC) == 0x1);
            radSortC3.Checked = ((spec.ufCond.SortC) == 0x2);

            radSortD1.Checked = ((spec.ufCond.SortD) == 0x0);
            radSortD2.Checked = ((spec.ufCond.SortD) == 0x1);
            radSortD3.Checked = ((spec.ufCond.SortD) == 0x2);

            radSortE1.Checked = ((spec.ufCond.SortE) == 0x0);
            radSortE2.Checked = ((spec.ufCond.SortE) == 0x1);
            radSortE3.Checked = ((spec.ufCond.SortE) == 0x2);

        }

        private void TabSorterGet(Spec spec)
        {
            spec.ufCond.SortA = 0;
            if (radSortA1.Checked == true)
                spec.ufCond.SortA = 0;
            if (radSortA2.Checked == true)
                spec.ufCond.SortA = 1;
            if (radSortA3.Checked == true)
                spec.ufCond.SortA = 2;

            spec.ufCond.SortB = 0;
            if (radSortB1.Checked == true)
                spec.ufCond.SortB = 0;
            if (radSortB2.Checked == true)
                spec.ufCond.SortB = 1;
            if (radSortB3.Checked == true)
                spec.ufCond.SortB = 2;

            spec.ufCond.SortC = 0;
            if (radSortC1.Checked == true)
                spec.ufCond.SortC = 0;
            if (radSortC2.Checked == true)
                spec.ufCond.SortC = 1;
            if (radSortC3.Checked == true)
                spec.ufCond.SortC = 2;

            spec.ufCond.SortD = 0;
            if (radSortD1.Checked == true)
                spec.ufCond.SortD = 0;
            if (radSortD2.Checked == true)
                spec.ufCond.SortD = 1;
            if (radSortD3.Checked == true)
                spec.ufCond.SortD = 2;

            spec.ufCond.SortE = 0;
            if (radSortE1.Checked == true)
                spec.ufCond.SortE = 0;
            if (radSortE2.Checked == true)
                spec.ufCond.SortE = 1;
            if (radSortE3.Checked == true)
                spec.ufCond.SortE = 2;

        }

        private void TabRoBalCondSet(Spec spec)
        {
            //txtRro3ch.Text = spec.roCond.Rro3ch.ToString();
            cmbRro3ch.SelectedIndex = spec.roCond.Rro3ch;//.ToString();
            txtRoSmpAveNum.Text = spec.roCond.SmpAveNum.ToString();
            txtRoWaveUse.Text = spec.roCond.WaveUse.ToString();

            cmbDBUnit.SelectedIndex = spec.balCond.DBUnit;
            cmbMSUnit.SelectedIndex = spec.balCond.MSUnit;
            cmbMCUnit.SelectedIndex = spec.balCond.MCUnit;
            txtCalGainNo.Text = (spec.balCond.CalGainNo + 1).ToString();
            txtCalToolNo.Text = (spec.balCond.CalToolNo + 1).ToString();

        }

        private void TabRoBalCondGet(Spec spec)
        {
            //spec.roCond.Rro3ch = Convert.ToInt32(txtRro3ch.Text);
            spec.roCond.Rro3ch = cmbRro3ch.SelectedIndex;
            spec.roCond.SmpAveNum = Convert.ToInt32(txtRoSmpAveNum.Text);
            spec.roCond.WaveUse = Convert.ToInt32(txtRoWaveUse.Text);

            spec.balCond.DBUnit = cmbDBUnit.SelectedIndex;
            spec.balCond.MSUnit = cmbMSUnit.SelectedIndex;
            spec.balCond.MCUnit = cmbMCUnit.SelectedIndex;
            spec.balCond.CalGainNo = Convert.ToInt32(txtCalGainNo.Text) - 1;
            spec.balCond.CalToolNo = Convert.ToInt32(txtCalToolNo.Text) - 1;

        }

        private void TabBalCorrSet(Spec spec)
        {
            try
            {
                txtRimOffset.Text = spec.balCorr.RimOffset.ToString();
                txtUpWgDia.Text = spec.balCorr.UpWgDia.ToString();
                txtLoWgDia.Text = spec.balCorr.LoWgDia.ToString();

                txtUpWgDist.Text = spec.balCorr.UpWgDist.ToString();
                txtLoWgDist.Text = spec.balCorr.LoWgDist.ToString();
                txtCorrWgPit.Text = spec.balCorr.CorrWgPit.ToString();
                cmbUpPaste.SelectedIndex = (int)spec.balCorr.UpPaste;
                cmbLoPaste.SelectedIndex = (int)spec.balCorr.LoPaste;
                txtToolNum.Text = (spec.balCorr.ToolNum).ToString();
            }
            catch (NullReferenceExceptione)
            {
                spec.balCorr = new BalCorr();
            }

        }

        private void TabBalCorrGet(Spec spec)
        {
            spec.balCorr.RimOffset = Convert.ToSingle(txtRimOffset.Text);
            spec.balCorr.UpWgDia = Convert.ToSingle(txtUpWgDia.Text);
            spec.balCorr.LoWgDia = Convert.ToSingle(txtLoWgDia.Text);

            spec.balCorr.UpWgDist = Convert.ToSingle(txtUpWgDist.Text);
            spec.balCorr.LoWgDist = Convert.ToSingle(txtLoWgDist.Text);
            spec.balCorr.CorrWgPit = Convert.ToSingle(txtCorrWgPit.Text);
            spec.balCorr.UpPaste = cmbUpPaste.SelectedIndex;
            spec.balCorr.LoPaste = cmbLoPaste.SelectedIndex;
            spec.balCorr.ToolNum = Convert.ToInt32(txtToolNum.Text);
        }
        //============grinder =========================
        private void TabGrindSet(Spec spec)
        {
            lblGrindModelNo.Text = (s_ModelNo + 1).ToString();
            txtGrindCycleMax.Text = spec.grind.CycleMax.ToString();
            txtGrindPush.Text = spec.grind.Push.ToString();
            txtGrindHiLevel.Text = spec.grind.HiLevel.ToString();
            txtGrindLowLevel.Text = spec.grind.LowLevel.ToString();
            chkGrindUse.Checked = (spec.grind.Use != 0);
            chkGrindOABuf.Checked = (spec.grind.OABuf != 0);
            chkGrindFstBuf.Checked = (spec.grind.FstBuf != 0);
        }

        private void TabGrindGet(Spec spec)
        {
            spec.grind.CycleMax = Convert.ToInt32(txtGrindCycleMax.Text);
            spec.grind.Push = Convert.ToSingle(txtGrindPush.Text);
            spec.grind.HiLevel = Convert.ToSingle(txtGrindHiLevel.Text);
            spec.grind.LowLevel = Convert.ToSingle(txtGrindLowLevel.Text);
            spec.grind.OABuf = Convert.ToInt32(chkGrindOABuf.Checked);
            spec.grind.FstBuf = Convert.ToInt32(chkGrindFstBuf.Checked);
            spec.grind.Use = Convert.ToInt32(chkGrindUse.Checked);

        }
        //============ position data ===================
        private void TabPosDataSet(Spec spec)
        {
            lblPosModelNo.Text = (s_ModelNo + 1).ToString();
            txtWaitPos.Text = spec.ufCond.ufPos.WaitPos.ToString();
            txtSkimPos.Text = spec.ufCond.ufPos.SkimPos.ToString();
            txtLoadPos.Text = spec.ufCond.ufPos.LoadPos.ToString();

            cmbRroUnitFwd.SelectedIndex = spec.roCond.roSensPos.RroUnitFwd;
            txtRrotPos.Text = spec.roCond.roSensPos.RrotPos.ToString();
            txtRrocPos.Text = spec.roCond.roSensPos.RrocPos.ToString();
            txtRrobPos.Text = spec.roCond.roSensPos.RrobPos.ToString();
            cmbLroUnitDwn.SelectedIndex = spec.roCond.roSensPos.LroUnitDwn;
            txtLrotPos.Text = spec.roCond.roSensPos.LrotPos.ToString();
            txtLrobPos.Text = spec.roCond.roSensPos.LrobPos.ToString();

            txtGrindUpPos.Text = spec.grind.grindPos.UpPos.ToString();
            txtGrindLoPos.Text = spec.grind.grindPos.LoPos.ToString();
            txtGrindUpSkim.Text = spec.grind.grindPos.UpSkim.ToString();
            txtGrindLoSkim.Text = spec.grind.grindPos.LoSkim.ToString();

            txtUpMarkDia.Text = spec.ufCond.mkPos.UpMarkDia.ToString();
            txtLoMarkDia.Text = spec.ufCond.mkPos.LoMarkDia.ToString();

        }

        private void TabPosDataGet(Spec spec)
        {
            spec.ufCond.ufPos.WaitPos = Convert.ToSingle(txtWaitPos.Text);
            spec.ufCond.ufPos.SkimPos = Convert.ToSingle(txtSkimPos.Text);
            spec.ufCond.ufPos.LoadPos = Convert.ToSingle(txtLoadPos.Text);

            spec.roCond.roSensPos.RroUnitFwd = cmbRroUnitFwd.SelectedIndex;
            spec.roCond.roSensPos.RrotPos = Convert.ToSingle(txtRrotPos.Text);
            spec.roCond.roSensPos.RrocPos = Convert.ToSingle(txtRrocPos.Text);
            spec.roCond.roSensPos.RrobPos = Convert.ToSingle(txtRrobPos.Text);
            spec.roCond.roSensPos.LroUnitDwn = cmbLroUnitDwn.SelectedIndex;
            spec.roCond.roSensPos.LrotPos = Convert.ToSingle(txtLrotPos.Text);
            spec.roCond.roSensPos.LrobPos = Convert.ToSingle(txtLrobPos.Text);

            spec.grind.grindPos.UpPos = Convert.ToSingle(txtGrindUpPos.Text);
            spec.grind.grindPos.LoPos = Convert.ToSingle(txtGrindLoPos.Text);
            spec.grind.grindPos.UpSkim = Convert.ToSingle(txtGrindUpSkim.Text);
            spec.grind.grindPos.LoSkim = Convert.ToSingle(txtGrindLoSkim.Text);
            spec.grind.Push = Convert.ToSingle(txtGrindPush.Text);

            spec.ufCond.mkPos.UpMarkDia = Convert.ToSingle(txtUpMarkDia.Text);
            spec.ufCond.mkPos.LoMarkDia = Convert.ToSingle(txtLoMarkDia.Text);
        }

        private void TabSpecSet(int ModelNo)
        {
            //s_uUnit = Unit.uUnit[0, specAll[ModelNo].ufCond.Unit];
            s_uUnit = Unit.uUnit[0, Sys.UfmUnit];
            TabModelSet(specAll[ModelNo]);
            TabUfRankSet(specAll[ModelNo]);
            TabUfJudgSet(specAll[ModelNo]);
            TabRoRankSet(specAll[ModelNo]);
            TabRoJudgSet(specAll[ModelNo]);
            TabBalRankSet(specAll[ModelNo]);
            TabBalJudgSet(specAll[ModelNo]);
            TabUfhRankSet(specAll[ModelNo]);
            TabUfhJudgSet(specAll[ModelNo]);
            TabTestUseSet(specAll[ModelNo]);
            TabUfCondSet(specAll[ModelNo]);
            TabUfhCondSet(specAll[ModelNo]);
            TabUfMarkSet(specAll[ModelNo]);
            TabBalMarkSet(specAll[ModelNo]);
            TabSorterSet(specAll[ModelNo]);
            TabRoBalCondSet(specAll[ModelNo]);
            TabBalCorrSet(specAll[ModelNo]);
            TabCoefSet(specAll[ModelNo]);
            TabUfhCoefSet(specAll[ModelNo]);
            TabPosDataSet(specAll[ModelNo]);
            TabGrindSet(specAll[ModelNo]);

        }

        private void TabSpecGet(int ModelNo)
        {
            TabModelGet(specAll[ModelNo]);
            TabUfRankGet(specAll[ModelNo]);
            TabUfJudgGet(specAll[ModelNo]);
            TabRoRankGet(specAll[ModelNo]);
            TabRoJudgGet(specAll[ModelNo]);
            TabBalRankGet(specAll[ModelNo]);
            TabBalJudgGet(specAll[ModelNo]);
            TabUfhRankGet(specAll[ModelNo]);
            TabUfhJudgGet(specAll[ModelNo]);
            TabTestUseGet(specAll[ModelNo]);
            TabUfCondGet(specAll[ModelNo]);
            TabUfhCondGet(specAll[ModelNo]);
            TabUfMarkGet(specAll[ModelNo]);
            TabBalMarkGet(specAll[ModelNo]);
            TabSorterGet(specAll[ModelNo]);
            TabRoBalCondGet(specAll[ModelNo]);
            TabBalCorrGet(specAll[ModelNo]);
            TabCoefGet(specAll[ModelNo]);
            TabUfhCoefGet(specAll[ModelNo]);
            TabPosDataGet(specAll[ModelNo]);
            TabGrindGet(specAll[ModelNo]);

        }

        private void TabSpecUseSet()
        {
            //パスワードで設定有効無効をセット
            if (PassWd.PasswordOK < 2)
            {
                groupBoxUfmSpec.Enabled = false;
                groupBoxRoSpec.Enabled = false;
                groupBoxBalSpec.Enabled = false;
                groupBoxUfhSpec.Enabled = false;

                groupBoxTestUse.Enabled = false;

                groupBoxUfmDiff.Enabled = false;
                groupBoxRoDiff.Enabled = false;
                groupBoxBalDiff.Enabled = false;
                groupBoxUfhDiff.Enabled = false;

            }
            else
            {
                groupBoxUfmSpec.Enabled = true;
                groupBoxRoSpec.Enabled = true;
                groupBoxBalSpec.Enabled = true;
                groupBoxUfhSpec.Enabled = true;

                groupBoxTestUse.Enabled = true;

                groupBoxUfmDiff.Enabled = false;
                groupBoxRoDiff.Enabled = false;
                groupBoxBalDiff.Enabled = false;
                groupBoxUfhDiff.Enabled = false;

            }

            //sysconf の設定で無効をセット
            if (Sys.UfmUse == 0)
            {
                groupBoxUfmSpec.Enabled = false;
                groupBoxUfmDiff.Enabled = false;
            }

            if (Sys.UfhUse == 0)
            {
                groupBoxUfhSpec.Enabled = false;
                groupBoxUfhMeas.Enabled = false;
                groupBoxUfhDiff.Enabled = false;
            }

            if (Sys.UfmUse == 0 && Sys.UfhUse == 0) //低速高速共に使用しない
            {
                groupBoxUfmMeas.Enabled = false;
                groupBoxUfmPos.Enabled = false;
                groupBoxUfmMark.Enabled = false;
                groupBoxUfmMkSel.Enabled = false;
                groupBoxUfmMkPoint.Enabled = false;
                groupBoxUfmCon.Enabled = false;
            }

            if (Sys.RoUse == 0)
            {
                groupBoxRoSpec.Enabled = false;
                groupBoxRoMeas.Enabled = false;
                groupBoxRoPos.Enabled = false;
                groupBoxRoDiff.Enabled = false;
            }

            if (Sys.BalUse == 0)
            {
                groupBoxBalSpec.Enabled = false;
                groupBoxBalMeas.Enabled = false;
                groupBoxBalDiff.Enabled = false;
                groupBoxBalMark.Enabled = false;
                groupBoxBalMkSel.Enabled = false;
                groupBoxBalMkPoint.Enabled = false;
            }

            if (Sys.GrindUse == 0)
            {
                groupBoxGrind.Enabled = false;
                groupBoxGrindPos.Enabled = false;
            }
            //--- Marking & Sorter ------
            if (Sys.UfMarkPinUse == 0)
            {
                groupBoxUfmMark.Enabled = false;
            }
            if (Sys.UfMarkSelUse == 0)
            {
                groupBoxUfmMkSel.Enabled = false;
            }
            if (Sys.UfMarkHiUse == 0)
            {
                groupBoxUfmMkPoint.Enabled = false;
            }
            if (Sys.CONMarkDirUse == 0)
            {
                groupBoxUfmCon.Enabled = false;
            }
            if (Sys.DbMarkPinUse == 0)
            {
                groupBoxBalMark.Enabled = false;
            }
            if (Sys.DbMarkSelUse == 0)
            {
                groupBoxBalMkSel.Enabled = false;
            }
            if (Sys.DbMarkHiUse == 0)
            {
                groupBoxBalMkPoint.Enabled = false;
            }
            if (Sys.SorterUse == 0)
            {
                groupBoxSort.Enabled = false;
            }
            if (Sys.MarkOptUse == 0)
            {
                groupBoxMarkOpt.Enabled = false;
            }
        }

        //規格の例外処理
        private void SpecException(Spec[] specAll)
        {
            int i;

            if (Sys.UfMarkHiUse == 0)       //UF Mark Hi を使わない場合ハイポイントに固定
            {
                for (i = 0; i < Sys.RankMax; i++)
                    specAll[i].ufCond.MarkHL = 1;
            }
            if (Sys.DbMarkHiUse == 0)       //Bal Mark Hiを使わない場合はローポイントに固定
            {
                for (i = 0; i < Sys.RankMax; i++)
                    specAll[i].balCond.MarkHL = 0;
            }

        }

        public static string ConvStr(float dt, string Fmt)
        {
            try
            {
                return dt.ToString(Fmt);
            }
            catch
            {
                return "0";
            }
        }

        private void SpecForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    if (F1key)
                        modelSetClick();
    
                    break;

                case Keys.F2:
                    if (F2key)
                        modelCopyClick();
                    
                    break;

                case Keys.F3:
                    if (F3key)
                        modelPasteClick();
    
                    break;

                case Keys.F4:
                    if (F4key)
                        modelDelClick();
    
                    break;

                case Keys.F7:
                    if (F7key)
                        btnSpecCsv_Click(btnSpecCsv, new System.EventArgs());

                    break;

                case Keys.F8:
                    btnPassWord_Click(btnPassWord, new System.EventArgs());
                    break;

                case Keys.F9:
                    if (F9key)
                    {
                        saveClick();
                        AppEnd();
                    }
                    break;

                case Keys.F10:
                    //if (F10key)
                        AppEnd();

                    break;

            }
        }

        //gcmSel=0:g, 1:gcm, 2:gcmcm, 3:oz, 4:ozin 5:ozinin
        //dbSel 0:static, couple, 1:MLside, 2:MR, 3=MT
        public static Unit_t BalUnitSel(int gcmSel, Spec spec)
        {
            Unit_t unit = new Unit_t();
            //float Haba = 0;
            //static, couple はbead 位置で出力する。
            //switch (Sys.BalPoint)
            //{
            //    case 0:     //bead point
            //        //Ldia = spec.size.Bdia;  //unuse
            //        //Rdia = spec.size.Bdia;  //unuse
            //        Haba = spec.size.Haba;  //use
            //        break;

            //    case 1:     //weight point
            //        //Ldia = spec.balCorr.UpWgDia;    //unuse
            //        //Rdia = spec.balCorr.LoWgDia;    //unuse
            //        Haba = spec.size.Haba;
            //        //Haba = -spec.balCorr.UpWgDist + spec.size.Haba - spec.balCorr.LoWgDist;
            //        break;
            //}

            switch (gcmSel)
            {
                case 1: //gcm output
                    unit.Ka = Unit.bUnit[0, 1].Ka; //*spec.size.Bdia / 20;
                    unit.Str = Unit.bUnit[0, 1].Str;
                    unit.Fmt = Unit.bUnit[0, 1].Fmt;
                    break;
                case 2: //gcmcm
                    unit.Ka = Unit.bUnit[0, 2].Ka; // *(spec.size.Bdia / 20) * (Haba / 10);
                    unit.Str = Unit.bUnit[0, 2].Str;
                    unit.Fmt = Unit.bUnit[0, 2].Fmt;
                    break;
                case 3: //oz
                    unit.Ka = Unit.bUnit[1, 0].Ka;
                    unit.Str = Unit.bUnit[1, 0].Str;
                    unit.Fmt = Unit.bUnit[1, 0].Fmt;
                    break;
                case 4: //ozin
                    unit.Ka = Unit.bUnit[1, 1].Ka; // *spec.size.Bdia / 2;
                    unit.Str = Unit.bUnit[1, 1].Str;
                    unit.Fmt = Unit.bUnit[1, 1].Fmt;
                    break;
                case 5: //ozinin
                    unit.Ka = Unit.bUnit[1, 2].Ka; // *(spec.size.Bdia / 20) * (Haba / 10);
                    unit.Str = Unit.bUnit[1, 2].Str;
                    unit.Fmt = Unit.bUnit[1, 2].Fmt;
                    break;
                case 0: //g
                default:
                    unit.Ka = Unit.bUnit[0, 0].Ka;     //bead point coeff.
                    unit.Str = Unit.bUnit[0, 0].Str;
                    unit.Fmt = Unit.bUnit[0, 0].Fmt;
                    break;
            }
            return unit;
        }

        public string UfUnitStr(int unit)
        {
            if (unit == 0)
                return "N";
            else
                return "kgf";
        }

        private void txtModel_TextChanged(object sender, EventArgs e)
        {

        }

        //Ro rank
        private void lblRoRankUnit_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    [Serializable]
    internal class NullReferenceExceptione : Exception
    {
        public NullReferenceExceptione()
        {
        }

        public NullReferenceExceptione(string message) : base(message)
        {
        }

        public NullReferenceExceptione(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NullReferenceExceptione(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
    


