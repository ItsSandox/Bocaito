using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Firebase.Messaging;
using Bocaito.Services;
using System.Threading.Tasks;
using AndroidX.Core.App;
using Color = Android.Graphics.Color;


namespace Bocaito.Platforms.Android.Services
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseService : FirebaseMessagingService
    {
        private const string TAG = "FirebaseService";

        public override async void OnNewToken(string token)
        {
            base.OnNewToken(token);
            Log.Debug(TAG, "Token de Firebase actualizado: " + token);
            try 
            {
                var supabaseService = new SupabaseService();
                await supabaseService.InitializeAsync();
                
                if (supabaseService.CurrentUser != null)
                {
                    await supabaseService.GuardarOActualizarTokenFirebase(token);
                    Log.Debug(TAG, "Token guardado exitosamente");
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, $"Error al procesar nuevo token: {ex.Message}");
            }
        }

        public override void OnMessageReceived(RemoteMessage message)
        {   
            base.OnMessageReceived(message);
            Log.Debug(TAG, "Mensaje recibido de Firebase");

            try
            {
                // Obtener datos del mensaje
                var data = message.Data;
                string titulo = "Pedido actualizado";
                string estado = null;
                string mensaje = "Tu pedido ha sido actualizado";
                
                // Obtener el estado del pedido si está presente
                if (data.TryGetValue("estado", out string estadoValor))
                {
                    estado = estadoValor;
                    mensaje = ObtenerMensajeEstado(estado);
                    titulo = ObtenerTituloEstado(estado);
                }
                
                // Si hay mensaje de notificación, usarlo
                if (message.GetNotification() != null)
                {
                    if (!string.IsNullOrEmpty(message.GetNotification().Title))
                        titulo = message.GetNotification().Title;
                    
                    if (!string.IsNullOrEmpty(message.GetNotification().Body))
                        mensaje = message.GetNotification().Body;
                }
                
                // Mostrar la notificación
                MostrarNotificacion(titulo, mensaje, estado);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, $"Error al procesar mensaje: {ex.Message}");
            }
        }

        private void MostrarNotificacion(string titulo, string mensaje, string estado)
        {
            // Crear intent para abrir la app al tocar la notificación
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, intent, 
                PendingIntentFlags.Immutable);

            // Configurar la notificación con el color correspondiente al estado
            var notificationBuilder = new NotificationCompat.Builder(this, CreateNotificationChannel())
                .SetContentTitle(titulo)
                .SetContentText(mensaje)
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(mensaje))
                .SetSmallIcon(Resource.Drawable.icon)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetVibrate(new long[] { 0, 250, 250, 250 });
            
            // Agregar color si hay un estado
            if (!string.IsNullOrEmpty(estado))
            {
                Color color = ObtenerColorEstado(estado);
                notificationBuilder.SetColor(color.ToArgb());
                notificationBuilder.SetColorized(true);
            }

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(DateTime.Now.Millisecond, notificationBuilder.Build());
        }

        private string ObtenerTituloEstado(string estado)
        {
            return estado switch
            {
                "pendiente" => "Pedido Recibido",
                "en preparación" => "En Preparación",
                "en camino" => "En Camino",
                "entregado" => "¡Entregado!",
                "cancelado" => "Cancelado",
                _ => "Estado Actualizado"
            };
        }

        private string ObtenerMensajeEstado(string estado)
        {
            return estado switch
            {
                "pendiente" => "Tu pedido ha sido recibido y está pendiente de confirmación.",
                "en preparación" => "¡Buenas noticias! Tu pedido está siendo preparado ahora mismo.",
                "en camino" => "Tu pedido ya está en camino y pronto llegará a tu dirección.",
                "entregado" => "¡Tu pedido ha sido entregado con éxito! ¡Buen provecho!",
                "cancelado" => "Lo sentimos, tu pedido ha sido cancelado. Contáctanos si necesitas más información.",
                _ => "El estado de tu pedido ha sido actualizado."
            };
        }

        private Color ObtenerColorEstado(string estado)
        {
            return estado switch
            {
                "pendiente" => Color.ParseColor("#FFA500"),      // Naranja
                "en preparación" => Color.ParseColor("#3498DB"), // Azul
                "en camino" => Color.ParseColor("#9B59B6"),      // Morado
                "entregado" => Color.ParseColor("#2ECC71"),      // Verde
                "cancelado" => Color.ParseColor("#E74C3C"),      // Rojo
                _ => Color.ParseColor("#808080")                 // Gris por defecto
            };
        }

        private string CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return "bocaito_pedidos";
            }

            string channelId = "bocaito_pedidos";
            string channelName = "Estado de Pedidos";
            
            var channel = new NotificationChannel(channelId, channelName, NotificationImportance.High);
            channel.Description = "Canal de notificaciones para estados de pedidos";
            channel.SetVibrationPattern(new long[] { 0, 250, 250, 250 });
            channel.EnableVibration(true);
            channel.EnableLights(true);
            channel.LightColor = Color.Blue.ToArgb();
            
            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
            
            return channelId;
        }
    }
}