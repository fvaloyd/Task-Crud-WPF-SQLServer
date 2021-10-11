using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskCrudWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection myConnectionSql;
        public MainWindow()
        {
            InitializeComponent();

            string myConnection = ConfigurationManager.ConnectionStrings["TaskCrudWPF.Properties.Settings.bbddC_PracticaConnectionString"].ConnectionString;

            myConnectionSql = new SqlConnection(myConnection);

            MuestraTarea();
        }

        private void MuestraTarea()
        {
            string consulta = "SELECT *,CONCAT(tarea, ' => ',descripcion) AS FULLTAREA FROM task_tabla";

            SqlDataAdapter adaptador = new SqlDataAdapter(consulta, myConnectionSql);

            using (adaptador)
            {
                DataTable TareaTabla = new DataTable();

                adaptador.Fill(TareaTabla);

                Lista_Tareas.DisplayMemberPath = "FULLTAREA";

                Lista_Tareas.SelectedValuePath = "Id";

                Lista_Tareas.ItemsSource = TareaTabla.DefaultView;
            }
        }
        private void InsertarTarea()
        {
            if(Tarea.Text == "" || Descripcion.Text == "")
            {
                MessageBox.Show("No se puede enviar el formulario vacio");
            }
            else
            {
                string consulta = "INSERT INTO task_tabla (tarea, descripcion) VALUES (@TAREA, @DESCRIPCION)";

                SqlCommand comandoInsertar = new SqlCommand(consulta, myConnectionSql);

                myConnectionSql.Open();

                comandoInsertar.Parameters.AddWithValue("@TAREA", Tarea.Text);
                comandoInsertar.Parameters.AddWithValue("@DESCRIPCION", Descripcion.Text);

                comandoInsertar.ExecuteNonQuery();

                myConnectionSql.Close();

                MuestraTarea();

                Tarea.Text = "";
                Descripcion.Text = "";
            }
            
        }

        //boton insertar tarea
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InsertarTarea();
        }

        private void Lista_Tareas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridBtn.Visibility = Visibility.Visible;
            if (Lista_Tareas.SelectedValue != null)
            {
                IdTarea.Text = $"Tarea: {Lista_Tareas.SelectedValue.ToString()}";
            }
            else gridBtn.Visibility = Visibility.Hidden;
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            string consulta = "DELETE FROM task_tabla WHERE Id=@taskId";

            SqlCommand comandoEliminar = new SqlCommand(consulta, myConnectionSql);

            myConnectionSql.Open();

            comandoEliminar.Parameters.AddWithValue("@taskId", Lista_Tareas.SelectedValue);

            comandoEliminar.ExecuteNonQuery();

            myConnectionSql.Close();

            MuestraTarea();
        }

        private void Actualizar_Click(object sender, RoutedEventArgs e)
        {
            btnSave.Visibility = Visibility.Hidden;
            btnSaveUpdate.Visibility = Visibility.Visible;

            string consulta = "SELECT * FROM task_tabla WHERE Id=@idTask";

            SqlCommand comandoObtener = new SqlCommand(consulta, myConnectionSql);

            SqlDataAdapter adaptador = new SqlDataAdapter(comandoObtener);

            using (adaptador)
            {
                comandoObtener.Parameters.AddWithValue("@idTask", Lista_Tareas.SelectedValue);

                DataTable tablaTask = new DataTable();

                adaptador.Fill(tablaTask);

                Descripcion.Text = tablaTask.Rows[0]["descripcion"].ToString();
                Tarea.Text = tablaTask.Rows[0]["tarea"].ToString();
            }


        }

        private void btnSaveUpdate_Click(object sender, RoutedEventArgs e)
        {
            if(Tarea.Text == "" || Descripcion.Text == "")
            {
                MessageBox.Show("No se puede actualizar el formulario en blanco");
            }
            else
            {
                string consulta = "UPDATE task_tabla SET tarea=@tarea, descripcion=@descripcion WHERE Id=" + (int)Lista_Tareas.SelectedValue;

                SqlCommand comandoActualizar = new SqlCommand(consulta, myConnectionSql);

                myConnectionSql.Open();

                comandoActualizar.Parameters.AddWithValue("@tarea", Tarea.Text);
                comandoActualizar.Parameters.AddWithValue("@descripcion", Descripcion.Text);

                comandoActualizar.ExecuteNonQuery();

                myConnectionSql.Close();

                btnSaveUpdate.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;

                Tarea.Text = "";
                Descripcion.Text = "";

                MuestraTarea();
            }
            
        }
    }
}
