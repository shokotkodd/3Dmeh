using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Lab4
{
    public partial class Form1 : Form
    {
        Device d3d;

        Mesh koleso1, koleso2, koleso3;
        Mesh nitka1, nitka2;
        Mesh gruz;

        float koef = 100f;
        float r = 30f;
        float omega = 0.1f;
        float t = 0f;
        float l1 = 0f;
        float l2 = 0f;
        
        Material mehanizmMaterial;
        Material nitkaMaterial;
        Material gruzMaterial;
        
        bool karkas = false;

        bool ris = false;
        bool dvig = false;
        bool stop = true;

        bool fl;

        List<CustomVertex.PositionColored> myList = new List<CustomVertex.PositionColored>();

        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Устанавливаем режим отображения трехмерной графики
                PresentParameters d3dpp = new PresentParameters();
                d3dpp.BackBufferCount = 1;
                d3dpp.SwapEffect = SwapEffect.Discard;
                d3dpp.Windowed = true; // Выводим графику в окно
                d3dpp.MultiSample = MultiSampleType.None; // Выключаем антиалиасинг
                d3dpp.EnableAutoDepthStencil = true; // Разрешаем создание z-буфера
                d3dpp.AutoDepthStencilFormat = DepthFormat.D16; // Z-буфер в 16 бит
                d3d = new Device(0, // D3D_ADAPTER_DEFAULT - видеоадаптер по 
                    // умолчанию
                      DeviceType.Hardware, // Тип устройства - аппаратный ускоритель
                      this, // Окно для вывода графики
                      CreateFlags.SoftwareVertexProcessing, // Геометрию обрабатывает CPU
                      d3dpp);

            }
            catch (Exception exc)
            {
                MessageBox.Show(this, exc.Message, "Ошибка инициализации");
                Close(); // Закрываем окно
            }

            koleso1 = Mesh.Cylinder(d3d, r / koef, r / koef, 0.1f, 25, 10);
            koleso2 = Mesh.Cylinder(d3d, r / 2 / koef, r / 2 / koef, 0.1f, 25, 10);
            koleso3 = Mesh.Cylinder(d3d, 3 * r / 4 / koef, 3 * r / 4 / koef, 0.2f, 25, 10);
            gruz = Mesh.Cylinder(d3d, r / 4 / koef, r / 4 / koef, 0.3f, 10, 10);
            nitka1 = Mesh.Cylinder(d3d, 0.5f / koef, 0.5f / koef, (3 * r / 2 + omega * r * t) / koef, 10, 10);
            nitka2 = Mesh.Cylinder(d3d, 0.5f / koef, 0.5f / koef, (4 * r - omega * r * t / 4) / koef, 10, 10);

            mehanizmMaterial = new Material();
            mehanizmMaterial.Diffuse = Color.DarkRed;
            mehanizmMaterial.Specular = Color.White;
            nitkaMaterial = new Material();
            nitkaMaterial.Diffuse = Color.Beige;
            nitkaMaterial.Specular = Color.White;
            gruzMaterial = new Material();
            gruzMaterial.Diffuse = Color.Brown;
            gruzMaterial.Specular = Color.White;


        }

        private void SetupCamera()
        {
            // Устанавливаем параметры источника освещения
            // Устанавливаем параметры источника освещения
            d3d.Lights[0].Enabled = true;   // Включаем нулевой источник освещения
            d3d.Lights[0].Diffuse = Color.White;         // Цвет источника освещения
            d3d.Lights[0].Position = new Vector3(0, 0, 0); // Задаем координаты
            d3d.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, this.Width / this.Height, 1.0f, 50.0f);
        }

        public void OnIdle(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Nachalo()
        {
            l1 = (3 * r / 2 + omega * r * t) / koef;
            l2 = (4 * r - omega * r * t / 4) / koef;

            d3d.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Green, 1.0f, 0);
            d3d.BeginScene();
            if (ris)
            {
                SetupCamera();
                if (karkas)
                    d3d.RenderState.FillMode = FillMode.WireFrame;
                else
                    d3d.RenderState.FillMode = FillMode.Solid;

                koleso1.Dispose();
                koleso2.Dispose();
                koleso3.Dispose();
                koleso1 = Mesh.Cylinder(d3d, r / koef, r / koef, 0.1f, 25, 10);
                koleso2 = Mesh.Cylinder(d3d, r / 2 / koef, r / 2 / koef, 0.1f, 25, 10);
                koleso3 = Mesh.Cylinder(d3d, 3 * r / 4 / koef, 3 * r / 4 / koef, 0.2f, 25, 10);

                d3d.Material = mehanizmMaterial;
                d3d.Transform.World = Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                koleso1.DrawSubset(0);
                d3d.Transform.World = Matrix.Translation(0, 0, -0.1f) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                koleso2.DrawSubset(0);
                d3d.Transform.World = Matrix.Translation(r / 4 / koef, -(4 * r - omega * r * t / 4) / koef, -0.05f) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                koleso3.DrawSubset(0);

                nitka1.Dispose();
                nitka2.Dispose();
                nitka1 = Mesh.Cylinder(d3d, 0.5f / koef, 0.5f / koef, l1, 10, 10);
                nitka2 = Mesh.Cylinder(d3d, 0.5f / koef, 0.5f / koef, l2, 10, 10);

                d3d.Material = nitkaMaterial;
                d3d.Transform.World = Matrix.RotationX((float)Math.PI * 90 / 180) * Matrix.Translation((-r / 2 + 1f) / koef, -(4 * r - omega * r * t / 4) / koef / 2, -0.1f) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                nitka2.DrawSubset(0);
                d3d.Transform.World = Matrix.RotationX((float)Math.PI * 90 / 180) * Matrix.Translation((r - 1f) / koef, -(4 * r - omega * r * t / 4) / koef / 2, 0) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                nitka2.DrawSubset(0);
                d3d.Transform.World = Matrix.RotationX((float)Math.PI * 90 / 180) * Matrix.Translation(-(r - 1f) / koef, -(3 * r / 2 + omega * r * t) / koef / 2, 0) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                nitka1.DrawSubset(0);

                gruz.Dispose();
                gruz = Mesh.Cylinder(d3d, r / 4 / koef, r / 4 / koef, 0.3f, 10, 10);

                d3d.Material = gruzMaterial;
                d3d.Transform.World = Matrix.RotationX((float)Math.PI * 90 / 180) * Matrix.Translation(-(r - 1f) / koef, -(3 * r / 2 + omega * r * t) / koef - 0.15f, 0) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                gruz.DrawSubset(0);

            }
            d3d.EndScene();
            //Показываем содержимое дублирующего буфера
            d3d.Present();
        }

        private void Dvigenie()
        {
            l1 = (3 * r / 2 + omega * r * t) / koef;
            l2 = (4 * r - omega * r * t / 4) / koef;

            d3d.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Green, 1.0f, 0);
            //SetupProekcii();
            d3d.BeginScene();
            if (ris)
            {
                SetupCamera();
                if (karkas)
                    d3d.RenderState.FillMode = FillMode.WireFrame;
                else
                    d3d.RenderState.FillMode = FillMode.Solid;

                if (dvig)
                {
                    koleso1.Dispose();
                    koleso2.Dispose();
                    koleso3.Dispose();
                    koleso1 = Mesh.Cylinder(d3d, r / koef, r / koef, 0.1f, 25, 10);
                    koleso2 = Mesh.Cylinder(d3d, r / 2 / koef, r / 2 / koef, 0.1f, 25, 10);
                    koleso3 = Mesh.Cylinder(d3d, 3 * r / 4 / koef, 3 * r / 4 / koef, 0.2f, 25, 10);

                    d3d.Material = mehanizmMaterial;
                    d3d.Transform.World = Matrix.RotationZ(omega * t) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                    koleso1.DrawSubset(0);
                    d3d.Transform.World = Matrix.RotationZ(omega * t) * Matrix.Translation(0, 0, -0.1f) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                    koleso2.DrawSubset(0);
                    d3d.Transform.World = Matrix.RotationZ(-(float)(3.0 * omega * r / 4.0 * t)) * Matrix.Translation(r / 4 / koef, -(4 * r - omega * r * t / 4) / koef, -0.05f) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                    koleso3.DrawSubset(0);

                    nitka1.Dispose();
                    nitka2.Dispose();
                    nitka1 = Mesh.Cylinder(d3d, 0.5f / koef, 0.5f / koef, l1, 10, 10);
                    nitka2 = Mesh.Cylinder(d3d, 0.5f / koef, 0.5f / koef, l2, 10, 10);

                    d3d.Material = nitkaMaterial;
                    d3d.Transform.World = Matrix.RotationX((float)Math.PI * 90 / 180) * Matrix.Translation((-r / 2 + 1f) / koef, -(4 * r - omega * r * t / 4) / koef / 2, -0.1f) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                    nitka2.DrawSubset(0);
                    d3d.Transform.World = Matrix.RotationX((float)Math.PI * 90 / 180) * Matrix.Translation((r - 1f) / koef, -(4 * r - omega * r * t / 4) / koef / 2, 0) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                    nitka2.DrawSubset(0);
                    d3d.Transform.World = Matrix.RotationX((float)Math.PI * 90 / 180) * Matrix.Translation(-(r - 1f) / koef, -(3 * r / 2 + omega * r * t) / koef / 2, 0) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                    nitka1.DrawSubset(0);

                    gruz.Dispose();
                    gruz = Mesh.Cylinder(d3d, r / 4 / koef, r / 4 / koef, 0.3f, 10, 10);

                    d3d.Material = gruzMaterial;
                    d3d.Transform.World = Matrix.RotationX((float)Math.PI * 90 / 180) * Matrix.Translation(-(r - 1f) / koef, -(3 * r / 2 + omega * r * t) / koef - 0.15f, 0) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                    gruz.DrawSubset(0);

                    if (checkBox1.Checked)
                    {
                        float x = (float)((r / 4.0 - 3.0 * r / 4.0 + 3.0 * r / 4.0 - 3.0 * r / 4.0 * Math.Sin(omega * t)) / koef);
                        float y = -(float)((4 * r - 3 * r / 4 - omega * r / 4 * t + 3 * r / 4 - 3 * r / 4 * Math.Cos(omega * t)) / koef);
                        float z = 0f;
                        CustomVertex.PositionColored one = new CustomVertex.PositionColored();
                        one.Position = new Vector3(x, y, z);
                        one.Color = Color.Black.ToArgb();
                        myList.Add(one);



                        d3d.VertexFormat = CustomVertex.PositionColored.Format;
                        CustomVertex.PositionColored[] verts = new CustomVertex.PositionColored[myList.Count];
                        for (int i = 0; i < myList.Count; i++)
                            verts[i] = myList[i];
                        if (verts.Length > 2)
                        {

                            d3d.Transform.World = Matrix.Translation(0, 0, -0.05f) * Matrix.RotationY((float)(hScrollBar1.Value * Math.PI / 180)) * Matrix.Translation(0, 0.5f, 4f);
                            d3d.DrawUserPrimitives(PrimitiveType.LineStrip, verts.Length - 1, verts);
                        }
                    }

                    if (l1 <= r / 2 / koef) fl = true;
                    if (l2 <= 1.75 * r / koef) fl = false;
                    if (stop)
                    {
                        if (omega > 0)
                        {
                            if (fl) t += 0.1f;
                            if (!fl) t -= 0.1f;
                        }
                        if (omega < 0)
                        {
                            if (fl) t -= 0.1f;
                            if (!fl) t += 0.1f;
                        }
                    }
                }

            }
            d3d.EndScene();
            //Показываем содержимое дублирующего буфера
            d3d.Present();
        }

        private void Traekt()
        {
            d3d.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Green, 1.0f, 0);

            float x = (float)((r / 4.0 - 3.0 * r / 4.0 + 3.0 * r / 4.0 - 3.0 * r / 4.0 * Math.Sin(omega * t) / koef));
            float y = (float)((4 * r - 3 * r / 4 - omega * r / 4 * t + 3 * r / 4 - 3 * r / 4 * Math.Cos(omega * t)) / koef);
            float z = 4f;
            CustomVertex.PositionColored one = new CustomVertex.PositionColored();
            one.Position = new Vector3(x, y, z);
            myList.Add(one);

            d3d.VertexFormat = CustomVertex.PositionColored.Format;
            CustomVertex.PositionColored[] verts = new CustomVertex.PositionColored[myList.Count];
            for (int i = 0; i < myList.Count; i++)
                verts[i] = myList[i];

            if (verts.Length > 2)
                d3d.DrawUserPrimitives(PrimitiveType.LineStrip, verts.Length - 1, verts);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (dvig) Dvigenie();
            else Nachalo();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ris = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ris = false;
            dvig = false;
            t = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dvig = true;
            stop = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            stop = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            karkas = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            karkas = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            omega = (float)Convert.ToDouble(maskedTextBox1.Text.Substring(0, 4));
        }

        private void numericUpDown1_Click(object sender, EventArgs e)
        {
            r = (float)numericUpDown1.Value;
            numericUpDown2.Value = Convert.ToInt32(r / 2);
            numericUpDown3.Value = Convert.ToInt32(3 * r / 4);
        }

        private void numericUpDown2_Click(object sender, EventArgs e)
        {
            r = (float)(2 * numericUpDown2.Value);
            numericUpDown1.Value = Convert.ToInt32(r);
            numericUpDown3.Value = Convert.ToInt32(3 * r / 4);
        }

        private void numericUpDown3_Click(object sender, EventArgs e)
        {
            r = (float)(4 * numericUpDown3.Value / 3);
            numericUpDown1.Value = Convert.ToInt32(r);
            numericUpDown2.Value = Convert.ToInt32(r / 2);
        }
    }
}
