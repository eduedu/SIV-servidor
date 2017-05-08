﻿using System;
using System.Collections.Generic;
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
using SIV_Servidor;
using System.Data;
using System.Data.SQLite;
using System.Collections.ObjectModel;

namespace SIV_Servidor
{
    public partial class ucCaja : UserControl
    {
        ///Globales 
        public static ObservableCollection<itemCaja> mArticulosCaja = 
            new ObservableCollection<itemCaja>();   //Lista los articulos de la caja

        ///MAIN
        public ucCaja()
        {
            InitializeComponent();
            listCaja.ItemsSource = mArticulosCaja;

            ActualiarCajaDesdeDB();
        }

        //------------------------------------------------------------------------------------------
        ///-----------------------------------------------------------------------------------------

        ///CONTROLES
        private void listCaja_GotFocus(object sender, RoutedEventArgs e)
        {
            ///ayuda(zAyuda.listCaja1);
            MainWindow.ayuda2(zAyuda.listCaja1);
            //MainWindow.consola(zAyuda.listCaja1);
            
        }
        private void listCaja_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var list = sender as ListView;

                var selected = list.SelectedItem;
                var fila = selected as itemCaja;

                /*
                string codigopro = ((fila.codigo == "" || fila.codigo == null) ? "" : fila.codigo.ToString());
                string descripcion = fila.descripcion.ToString();
                string precio = fila.precio.ToString("0.00");

                tbCodigo.Text = codigopro;
                seEditoDescripcionDesdeElPrograma = true;
                tbDescripcion.Text = descripcion;
                tbPrecio.Text = precio;
                tbPrecio.Tag = precio;
                //consola("costo:" + fila.costo.ToString());
                tbCantidad.Tag = fila.costo;
                //consola("tbPrecio.Tag:"+tbPrecio.Tag.ToString());

                tbCantidad.Text = "1";
                tbCantidad.SelectAll();
                tbCantidad.Focus();
                listFiltro.Visibility = Visibility.Hidden;
                */
                consola("Venta Nro:" + fila.idventa.ToString());
                MessageBox.Show("Crear campos (nombre, direccion, cuit, etc. Qu");
            }
        }




        ///Funciones caja
        public static void ActualiarCajaDesdeDB()
        {
            SQLiteConnection conexion;
            conexion = new SQLiteConnection("Data Source=caja.db;Version=3;New=False;Compress=True;");
            conexion.Open();

            string consulta = "select * from caja ORDER BY id DESC";
            //string consulta = "select * from caja";

            /// Adaptador de datos, DataSet y tabla
            SQLiteDataAdapter db = new SQLiteDataAdapter(consulta, conexion);
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            dataSet.Reset();
            db.Fill(dataSet);
            dataTable = dataSet.Tables[0];
            //dataGrid.DataSource = dt;
            //dataGrid.DataContext = dt.DefaultView;  //esto anda

            //gridFiltro.ItemsSource = dataTable.DefaultView;
            //listFiltro.ItemsSource = dataTable.DefaultView;

            ///borrar todos los elementos de mArticulos
            if (mArticulosCaja != null)
            {
                mArticulosCaja.Clear();
            }

            ///Loop por todos los registros de la tabla
            int idventaMostrar = -1;
            float tmpTotal = 0;
            int index = 0;
            foreach (DataRow registro in dataTable.Rows)
            {
                float cantidad = funciones.toFloat(registro["cantidad"].ToString());
                float precio = funciones.toFloat(registro["precio"].ToString());
                float tmpSubtotal = cantidad * precio;

                //consola(registro["precio"].ToString());

                int tmpidventa = 0;
                int.TryParse(registro["idventa"].ToString(), out tmpidventa);


                itemCaja tempArticulo = new itemCaja();
                if (tmpidventa != idventaMostrar)
                {
                    idventaMostrar = tmpidventa;
                    tempArticulo.idventamostrar = idventaMostrar.ToString();
                    tmpTotal = tmpSubtotal;
                }
                else
                {
                    tempArticulo.idventamostrar = "";
                    tmpTotal = tmpTotal + tmpSubtotal;
                }
                if ((tmpidventa % 2) == 0)
                {
                    tempArticulo.color = 0;
                }
                else
                {
                    tempArticulo.color = 1;
                }
                tempArticulo.idventa = tmpidventa;
                tempArticulo.codigo = registro["codigo"].ToString();
                tempArticulo.descripcion = registro["descripcion"].ToString();
                tempArticulo.cantidad = cantidad.ToString();
                tempArticulo.precio = precio.ToString("0.00");
                tempArticulo.costo = registro["costo"].ToString();
                tempArticulo.subtotal = tmpSubtotal.ToString("0.00");
                tempArticulo.total = "";
                if (dataTable.Rows.Count != index + 1)
                {
                    String tmpSiguienteId = dataTable.Rows[index + 1]["idventa"].ToString();
                    //consola(registro["idventa"].ToString() + "-" + tmpSiguienteId);
                    if (registro["idventa"].ToString() != tmpSiguienteId)
                    {
                        tempArticulo.total = "$" + tmpTotal.ToString("0.00");
                    }
                }
                else
                {
                    tempArticulo.total = tmpTotal.ToString();
                }
                mArticulosCaja.Add(tempArticulo);
                index++;
            }


            ///asigno la lista al control listCaja
            //listCaja.ItemsSource = mArticulosCaja;
            //mArticulosCaja.Reverse();

            ///cerrar conexion
            conexion.Close();
        }


        ///extensiones
        private void ayuda(string texto = "", string texto2 = "")
        {
            MainWindow.ayuda2(texto, texto2);
        }
        private float toFloat(string cadena)
        {
            return funciones.toFloat(cadena);
        }
        private bool esDecimal(Key key)
        {
            return funciones.esDecimal(key);
        }
        private void consola(string texto)
        {
            funciones.consola(texto);
        }

    }
}
