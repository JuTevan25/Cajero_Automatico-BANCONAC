using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Cajero_Automatico_BANCONAC
{
    class Program
    {
        static void Main(string[] args)
        {
           
            string pinGuardado = "", pinIngresado = "";
            int opcion = 0;

            Transaccion miCuentaClass = new Transaccion(DateTime.Now, "", 0, 0);
            List<CuentaBancaria> miCuenta = new List<CuentaBancaria>();

            if (File.Exists("datos.json"))
            {
                string json = File.ReadAllText("datosCuenta.json");
                List<CuentaBancaria> cargados = JsonSerializer.Deserialize<List<CuentaBancaria>>(json) ?? new List<CuentaBancaria>();
                if (cargados != null)
                {
                    miCuenta.AddRange(cargados);
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            do
            {
                Console.Write("Ingresa un PIN para ser guardado: ");
                Console.ResetColor();
                pinGuardado = (Console.ReadLine() ?? "");
                Console.Clear();

                if (string.IsNullOrWhiteSpace(pinGuardado))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: No puede dejar el campo vacío.");
                    Console.ResetColor();
                    continue;
                }

                if (!pinGuardado.All(char.IsDigit))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: El PIN debe contener solo números.");
                    Console.ResetColor();
                    continue;
                }

                if (pinGuardado.Length != 4)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: El PIN debe tener exactamente 4 dígitos.");
                    Console.ResetColor();
                    continue;
                }

                break;

            } while (true);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("PIN guardado.");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Por favor ingresa el PIN para acceder: ");
            Console.ResetColor();
            pinIngresado = (Console.ReadLine() ?? "");

            while (pinIngresado.Length < 4)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Solo es permitido 4 dígitos.");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Por favor ingresa el PIN para acceder: ");
                pinIngresado = (Console.ReadLine() ?? "");
            }

            while (pinIngresado != pinGuardado)
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: PIN incorrecto.");
                Console.ResetColor();
                pinIngresado = (Console.ReadLine() ?? "");

            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Acceso Concedido, ingresando....");
            Console.ResetColor();

            Console.WriteLine("");
            Console.WriteLine("BIENVENIDO AL CAJERO AUTOMÁTICO BANCONAC");
            Console.WriteLine("A continuación verá el menú de opciones..");
            Console.WriteLine("");

            do
            {
                Console.WriteLine("==== MENÚ DE OPCIONES ====");
                Console.WriteLine("1. Crear Cuenta");
                Console.WriteLine("2. Depositar dinero");
                Console.WriteLine("3. Retirar dinero");
                Console.WriteLine("4. Transferir dinero a otra cuenta");
                Console.WriteLine("5. Consultar Saldo");
                Console.WriteLine("6. Ver Historial");
                Console.WriteLine("7. Ver Cuentas Registradas");
                Console.WriteLine("8. Salir");
                Console.WriteLine("");
                Console.Write("Opción: ");
                try
                {
                    opcion = int.Parse(Console.ReadLine() ?? "");
                }
                catch (FormatException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Por favor ingresa un número válido.");
                    Console.ResetColor();
                    continue;
                }

                Console.Clear();

                switch (opcion)
                {
                    case 1:

                        int dig_cuenta = 0;
                        Console.WriteLine("CREAR CUENTA");
                        Console.WriteLine("Por favor ingresa un nombre para el titular de la cuenta: ");
                        string nom_titular = (Console.ReadLine() ?? "");

                        if (string.IsNullOrWhiteSpace(nom_titular) || nom_titular.Any(char.IsDigit))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: El nombre del titular no puede estar vacío ni contener dígitos.");
                            Console.ResetColor();
                            nom_titular = (Console.ReadLine() ?? "");
                        }

                        Console.WriteLine("");
                        Console.WriteLine("Por favor escribe el número de cuenta máximo de 5 dígitos, inicio de código 10000: ");
                        try
                        {
                            dig_cuenta = int.Parse(Console.ReadLine() ?? "");
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: Por favor ingresa un número válido.");
                            Console.ResetColor();
                            dig_cuenta = int.Parse(Console.ReadLine() ?? "");
                        }

                        if (dig_cuenta < 10000 || dig_cuenta > 99999)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: El número de cuenta debe ser mínimo del código 10000.");
                            Console.ResetColor();
                            dig_cuenta = int.Parse(Console.ReadLine() ?? "");
                        }

                        var existe_Numero = miCuenta.Find(n => n.numeroCuenta == dig_cuenta);

                        if (existe_Numero != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: El número de cuenta ya existe.");
                            Console.ResetColor();
                            dig_cuenta = int.Parse(Console.ReadLine() ?? "");

                        }

                        Console.WriteLine("");
                        Console.WriteLine("Por favor, escribe el saldo inicial.");
                        Console.Write("$: ");
                        double saldo_Cuenta = double.Parse(Console.ReadLine() ?? "");

                        if (saldo_Cuenta <= 50)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: El Saldo debe ser mayor o igual a $50 pesos.");
                            Console.Write("$: ");
                            saldo_Cuenta = double.Parse(Console.ReadLine() ?? "");
                        }
                        Console.WriteLine("");
                        Console.WriteLine("Por favor, escribe un PIN para ser reconocido.");
                        int pin_Cuenta = int.Parse(Console.ReadLine() ?? "");

                        if (pin_Cuenta == int.Parse(pinIngresado))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: El PIN de la cuenta no puede ser igual al PIN de acceso.");
                            Console.ResetColor();
                            pin_Cuenta = int.Parse(Console.ReadLine() ?? "");
                        }

                        miCuenta.Add(new CuentaBancaria(dig_cuenta, nom_titular, saldo_Cuenta, pin_Cuenta));

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("La cuenta ha sido creada exitosamente.");
                        Console.WriteLine("Oprime Enter para continuar.");
                        Console.ResetColor();
                        Console.WriteLine("");
                        Console.ReadKey();
                        Console.Clear();

                        string json = JsonSerializer.Serialize(miCuenta);
                        File.WriteAllText("datosCuenta.json", json);


                        break;

                    case 2:
                        Console.WriteLine("DEPOSITO");
                        Console.WriteLine("Por favor escribe el número de cuenta para depositar");
                        string numeroCuenta = Console.ReadLine() ?? "";
                        Console.WriteLine("");

                        if (!int.TryParse(numeroCuenta, out int numCuenta))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: Número de cuenta inválido.");
                            numeroCuenta = Console.ReadLine() ?? "";
                        }

                        CuentaBancaria? cuenta = miCuenta.Find(c => c.numeroCuenta == numCuenta);

                        if (cuenta == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Cuenta no encontrada. Por favor ingrese nuevamente.");
                            Console.ResetColor();
                            break;
                        }

                        Console.WriteLine("Por favor, escribe el PIN para validar la transacción: ");
                        int PIN = int.Parse(Console.ReadLine() ?? "");
                        Console.WriteLine("");

                        CuentaBancaria? buscarPIN = miCuenta.Find(p => p.GetPIN() == PIN);

                        if (cuenta.GetPIN() != PIN)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("PIN incorrecto.");
                            Console.ResetColor();
                            PIN = int.Parse(Console.ReadLine() ?? "");
                        }
                        else
                        {
                            Console.WriteLine("Por favor escribe el monto a depositar:");
                            Console.Write("$: ");
                            double monto = double.Parse(Console.ReadLine() ?? "");
                            cuenta.Deposito(monto);
                        }

                        Console.WriteLine("Oprime Enter para continuar.");
                        Console.ReadKey();
                        Console.Clear();
                        break;

                    case 3:
                        Console.WriteLine("RETIRO");
                        Console.WriteLine("Por favor escribe el número de cuenta para retirar");
                        string numeroCuentaRetiro = Console.ReadLine() ?? "";
                        Console.WriteLine("");

                        if (!int.TryParse(numeroCuentaRetiro, out numCuenta))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: Número de cuenta inválido.");
                            Console.ResetColor();
                            numeroCuentaRetiro = Console.ReadLine() ?? "";
                        }

                        CuentaBancaria? retiroCuenta = miCuenta.Find(c => c.numeroCuenta == numCuenta);
                        if (retiroCuenta == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Cuenta no encontrada.");
                            Console.ResetColor();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Por favor escribe el monto a retirar:");
                            Console.Write("$: ");
                            double monto = double.Parse(Console.ReadLine() ?? "");
                            retiroCuenta.Retiro(monto);
                        }

                        Console.WriteLine("");
                        Console.WriteLine("Oprime Enter para continuar.");
                        Console.ReadKey();
                        Console.Clear();
                        break;


                    case 4:
                        Console.WriteLine("TRANSFERENCIA A OTRA CUENTA");
                        Console.Write("Número de cuenta origen: ");
                        string numOrigenStr = Console.ReadLine() ?? "";
                        Console.WriteLine("");
                        if (!int.TryParse(numOrigenStr, out int numOrigen))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Número de cuenta inválido.");
                            Console.ResetColor();
                            numOrigenStr = Console.ReadLine() ?? "";
                        }

                        CuentaBancaria? cuentaOrigen = miCuenta.Find(c => c.numeroCuenta == numOrigen);
                        if (cuentaOrigen == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Cuenta origen no encontrada.");
                            Console.ResetColor();
                            break;
                        }
                        Console.WriteLine("");
                        Console.Write("Número de cuenta destino: ");
                        string numDestinoStr = Console.ReadLine() ?? "";

                        if (!int.TryParse(numDestinoStr, out int numDestino))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Número de cuenta inválido.");
                            Console.ResetColor();
                            numDestinoStr = Console.ReadLine() ?? "";
                        }

                        Console.WriteLine("");

                        CuentaBancaria? cuentaDestino = miCuenta.Find(c => c.numeroCuenta == numDestino);
                        if (cuentaDestino == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Cuenta destino no encontrada.");
                            Console.ResetColor();
                            break;
                        }

                        // Validar PIN de origen
                        Console.Write("Ingrese el PIN de la cuenta origen: ");
                        if (!int.TryParse(Console.ReadLine() ?? "", out int pinOrigen))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("PIN inválido.");
                            Console.ResetColor();
                            break;
                        }
                        Console.WriteLine("");
                        if (cuentaOrigen.GetPIN() != pinOrigen)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("PIN incorrecto.");
                            Console.ResetColor();
                            break;
                        }

                        Console.Write("Monto a transferir: ");
                        Console.Write("$: ");
                        if (!double.TryParse(Console.ReadLine() ?? "", out double montoTransferencia) || montoTransferencia <= 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Monto inválido.");
                            Console.ResetColor();
                            break;
                        }

                        if (montoTransferencia > cuentaOrigen.MostrarSaldo())
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Saldo insuficiente en la cuenta origen.");
                            Console.ResetColor();
                            break;
                        }

                        // Realizar la transferencia
                        cuentaOrigen.Retiro(montoTransferencia);   // descuenta de origen
                        cuentaDestino.Deposito(montoTransferencia); // suma a destino

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Transferencia realizada exitosamente.");
                        Console.ResetColor();
                        Console.WriteLine("Oprime Enter para continuar.");
                        Console.ReadKey();
                        Console.Clear();
                        break;


                    case 5:
                        Console.WriteLine("CONSULTAR SALDO");
                        Console.WriteLine("Por favor escribe el número de cuenta: ");
                        numeroCuenta = (Console.ReadLine() ?? "");
                        Console.WriteLine("");
                        if (!int.TryParse(numeroCuenta, out int numeroConsulta))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Número de cuenta inválido.");
                            Console.ResetColor();
                            break;
                        }

                        CuentaBancaria? consultaSaldo = miCuenta.Find(s => s.numeroCuenta == int.Parse(numeroCuenta));

                        if (consultaSaldo == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Cuenta no encontrada.");
                            Console.ResetColor();
                            break;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"Nombre del Titular: {consultaSaldo.GETNombreTitular()}");
                            Console.WriteLine($"El saldo de la cuenta {consultaSaldo.numeroCuenta} es $: {consultaSaldo.MostrarSaldo()}");
                            Console.ResetColor();
                        }

                        Console.WriteLine("");
                        Console.WriteLine("Oprime Enter para continuar.");
                        Console.ReadKey();
                        Console.Clear();
                        break;


                    case 6:
                        Console.WriteLine("HISTORIAL");
                        Console.WriteLine("Por favor escribe el número de cuenta: ");
                        numeroCuenta = (Console.ReadLine() ?? "");
                        Console.WriteLine("");

                        if (!int.TryParse(numeroCuenta, out int numeroConsultaHisto))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Número de cuenta inválido.");
                            Console.ResetColor();
                            break;
                        }

                        CuentaBancaria? consultaHistorial = miCuenta.Find(c => c.numeroCuenta == int.Parse(numeroCuenta));

                        if (consultaHistorial == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Cuenta no encontrada.");
                            Console.ResetColor();
                            break;
                        }
                        else
                        {
                            consultaHistorial.MostrarHisto();
                        }

                        Console.WriteLine("Oprime Enter para continuar.");
                        Console.ReadKey();
                        Console.Clear();

                        break;

                    case 7:
                        Console.WriteLine("CUENTAS REGISTRADAS");
                        foreach (var cr in miCuenta)
                        {
                            Console.WriteLine("=========================================");
                            Console.WriteLine($"Número de Cuenta: {cr.numeroCuenta}");
                            Console.WriteLine($"Nombre del Titular: {cr.GETNombreTitular()}");
                            Console.WriteLine($"Saldo $: {cr.MostrarSaldo()}");
                            Console.WriteLine($"PIN: No se muestra por motivos de seguridad");
                            Console.WriteLine("=========================================");
                            Console.WriteLine("");
                        }

                        if (miCuenta.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("No hay cuentas registradas.");
                            Console.ResetColor();
                        }
                        Console.WriteLine("Oprime Enter para continuar.");
                        Console.ReadKey();
                        Console.Clear();
                        break;

                }
            }
            while (opcion != 8);
        }
    }

    class CuentaBancaria
    {
        List<Transaccion> historial = new List<Transaccion>();

        public int numeroCuenta { get; set; }
        private string NombreTitular { get; set; }
        private double Saldo { get; set; }

        private double MontoRetiradoHoy { get; set; }

        private DateTime fechaUltimoRetiro = DateTime.Today;

        private int PIN { get; set; }

        public CuentaBancaria(int numeroCuenta, string nombreTitular, double saldo, int PIN)
        {
            this.numeroCuenta = numeroCuenta;
            this.NombreTitular = nombreTitular;
            this.Saldo = saldo >= 0 ? saldo : 0;
            this.PIN = PIN;
        }

        public int GetPIN() => PIN;

        public string GETNombreTitular() => NombreTitular;

        public double Deposito(double monto)
        {

            if (monto <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Solo se permiten números positivos.");
                Console.ResetColor();
                return Saldo;
            }

            Saldo += monto;

            historial.Add(new Transaccion(DateTime.Now, "Depósito", monto, Saldo));


            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("El depósito se ha realizado correctamente.");
            Console.ResetColor();

            return Saldo;
        }

        public double Retiro(double monto)
        {

            if (fechaUltimoRetiro.Date != DateTime.Today)
            {

                MontoRetiradoHoy = 0;
                fechaUltimoRetiro = DateTime.Today;
            }

            if (monto < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Solo se permiten números positivos.");
                Console.ResetColor();
                return Saldo;
            }

            if (monto > Saldo)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: El saldo es insuficiente para realizar un retiro.");
                Console.ResetColor();
                return Saldo;
            }

            MontoRetiradoHoy += monto;

            if (MontoRetiradoHoy + monto > 1000000)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Límite diario de retiro excedido. Solo puede retirar {1000000 - MontoRetiradoHoy} más hoy.");
                return Saldo;
            }

            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("El retiro se ha realizado correctamente.");
            Console.ResetColor();


            Saldo -= monto;
            historial.Add(new Transaccion(DateTime.Now, "Retiro", monto, Saldo));

            return Saldo;
        }

        public void MostrarHisto()
        {
            foreach (var m in historial)
            {
                Console.WriteLine("==============");
                Console.WriteLine("");
                m.MostrarResumen();
                Console.WriteLine("");
                Console.WriteLine("==============");
            }
        }

        public double MostrarSaldo() => Saldo;

    }
    class Transaccion
    {
        private DateTime fecha { get; set; }
        private string tipo { get; set; }
        private double monto { get; set; }
        public double saldoPosterior { get; set; }

        public Transaccion(DateTime fecha, string tipo, double monto, double saldoPosterior)
        {
            this.fecha = fecha;
            this.tipo = tipo;
            this.monto = monto;
            this.saldoPosterior = saldoPosterior;
        }

        public void MostrarResumen()
        {
            Console.WriteLine("Fecha de Transacción: {0} ", fecha);
            Console.WriteLine("Tipo de Transacción: {0}", tipo);
            Console.WriteLine("Monto $: {0}", monto);
            Console.WriteLine("Saldo Posterior $: {0} ", saldoPosterior);
        }

    }
}

