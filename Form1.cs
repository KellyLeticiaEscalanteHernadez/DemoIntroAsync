using System.Diagnostics;

namespace DemoIntroAsync
{
    public partial class Form1 : Form
    {
        HttpClient httpClient = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;

            var directorioActual=AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual,@"Imagenes\resultado-secuencial");
            var destinoBaseParalelo = Path.Combine(directorioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecución(destinoBaseParalelo, destinoBaseSecuencial);

            Console.WriteLine("inicio");
            List<Imagen> imagenes = ObtenerImagenes();

            //parte secuencial
            var sw = new Stopwatch();
            sw.Start();

            foreach (var imagen in imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencial,imagen);
            }
            Console.WriteLine("Secuencial  -  duración en segundos: {0}",sw.ElapsedMilliseconds/1000.0);
            sw.Restart();
            sw.Start();
            var tareasEnumerables = imagenes.Select(async imagenes =>
            {
                await ProcesarImagen(destinoBaseParalelo, imagenes);
            });

            //procesos lento
            var tareas = new List<Task>()
            {
                RealizarProcesamientoLargo(),
                RealizarProcesamientoLargoB(),
               RealizarProcesamientoLargoC()
            };
            await Task.WhenAll(tareas);
            

            var duracion = $"El programa se ejecuto en {sw.ElapsedMilliseconds / 1000.0} segundos";
            Console.WriteLine(duracion);

            var nombre = await ProcesamientoLargo();//método asincrono porque devuelve task, await espera de metodo asincrono
            MessageBox.Show($"Saludos,{nombre}");
            sw.Stop();
            pictureBox1.Visible = false;
        }
        private async Task ProcesarImagen(string directorio, Imagen imagen)
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();
            Bitmap bitmap;
            using (var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);
        }

        private static List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();
            for (int i = 0; i < 7; i++)
            {
                imagenes.Add
                (
                    new Imagen()
                    {
                        Nombre = $"Centro{i}.png",
                        URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/68/Plaza-barrios-san-salvador.png/309px-Plaza-barrios-san-salvador.png"
                    }
                );
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Cerén{i}.jpg",
                        URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/7a/ES_JoyadeCeren_06_2011_Estructura_9_Area_2_Tamazcal_2106_zoom_out.jpg/220px-ES_JoyadeCeren_06_2011_Estructura_9_Area_2_Tamazcal_2106_zoom_out.jpg"
                    }
                );
                imagenes.Add(
                   new Imagen()
                   {
                       Nombre = $"Congreso{i}.jpg",
                       URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/58/Pintura_de_Luis_Vergara_Ahumada._Muestra_al_diputado%2C_Jos%C3%A9_Sime%C3%B3n_Ca%C3%B1as_promulgando_su_famoso_discurso%2C_la_abolici%C3%B3n_de_la_esclavitud.jpg/200px-Pintura_de_Luis_Vergara_Ahumada._Muestra_al_diputado%2C_Jos%C3%A9_Sime%C3%B3n_Ca%C3%B1as_promulgando_su_famoso_discurso%2C_la_abolici%C3%B3n_de_la_esclavitud.jpg"
                   }
                );
            }
            return imagenes;
        }

        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecución(string destionoBaseParalelo, string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destionoBaseParalelo))
            {
                Directory.CreateDirectory(destionoBaseParalelo);
            }
            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory(destinoBaseSecuencial);
            }

            BorrarArchivos(destionoBaseParalelo);
            BorrarArchivos(destinoBaseSecuencial);
        }
   


        private async Task<string> ProcesamientoLargo()
        {
            await Task.Delay(1000);//asincrono
            return "Kelly";
        }
        private async Task RealizarProcesamientoLargo()
        {
            await Task.Delay(1000);//asincrono
            Console.WriteLine("Proceso A finalizado");
        }
        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000);//asincrono
            Console.WriteLine("Proceso  B finalizado");
        }
        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000);//asincrono
            Console.WriteLine("Proceso C finalizado");
        }
    }
}
