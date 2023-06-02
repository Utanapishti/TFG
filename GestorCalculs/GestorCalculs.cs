using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scripting;

namespace GestorCalculs
{
    public class GestorCalculs
    {
        private UInt32 timestamp = uint.MinValue;
        private bool timestampOverflow=false;        
        private Dictionary<string, List<Script>> scriptsPerParametres = new();
        GestorVariables variables;

        public GestorCalculs(ILogger logger, IOptions<GestorCalculsOptions> options)
        {
            //Read the configured scripts
            Dictionary<string, string> scriptsCodes = new();
            foreach (var scriptOption in options.Value.Scripts)
            {
                scriptsCodes.Add(scriptOption.Name, scriptOption.Code);
            }
            //and associate them by their parameters
            foreach (var calculatedValue in options.Value.CalculatedValues)
            {
                if (scriptsCodes.TryGetValue(calculatedValue.ScriptName, out var scriptCode))
                {
                    //Create the script class
                    ParametreScript[] parameters = new ParametreScript[calculatedValue.AssociatedParameters.Count()];

                    int index = 0;
                    foreach (var param in calculatedValue.AssociatedParameters)
                    {
                        parameters[index] = new ParametreScript()
                        {
                            VariableAssociada = param.AssociatedValue,
                            Nom = param.Name,
                        };                        
                    }

                    Script script = new Script()
                    {
                        CodiPython = scriptCode,
                        Parametres = parameters,
                    };

                    foreach (var parameter in parameters)
                    {
                        if (!scriptsPerParametres.ContainsKey(parameter.Nom))
                        {
                            scriptsPerParametres.Add(parameter.Nom, new List<Script>());
                        }

                        scriptsPerParametres[parameter.Nom].Add(script);
                    }
                }
                else
                {
                    logger.LogError($"Could not find script {calculatedValue.ScriptName}");
                }
            }
        }

        public void RebutDada(string nomVariable,double valor)
        {
            var ts = Interlocked.Increment(ref timestamp);
            if (ts==0)
            {
                //Overflow
            }

            var variable = variables.GetVariable(nomVariable);            

            var dada=variable.AfegirDada(ts, valor);

            CalculaRelacionats(variable.Nom, dada);
        }

        private void CalculaRelacionats(string nomVariable, Dada dada)
        {
            if (scriptsPerParametres.TryGetValue(nomVariable, out var scripts))
            {
                Parallel.ForEach(scripts, script =>
                {
                    //Demana calcul
                    //Envia el nom del script
                    //el ts de la dada rebuda
                    //el major ts dels paràmetres excepte la rebuda
                    var parametresCalcul = new
                    {
                        nomScript = script.Nom,
                        nomRebut = nomVariable,
                        valorRebut = dada.Valor,
                        tsRebut = dada.Timestamp,
                        tsActual = script.Parametres.Max(parametre => variables.GetLastTimestamp(parametre.VariableAssociada))
                    };
                });
            }
        }        
    }
}