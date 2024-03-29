﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scripting;

namespace GestorCalculs
{
    public class GestorFuncions 
    {
        private UInt32 _timestamp = uint.MinValue;
        private bool _timestampOverflow = false;
        private Dictionary<string, List<FuncioVariable>> _funcionsPerParametres = new();
        private Dictionary<string, FuncioVariable> _funcionsPerVariable = new();
        private GestorVariables _variables = new GestorVariables();
        private ILogger _logger;

        public delegate void DelPeticioCalcul(string variableCalcular, string variableRebuda, Dada dadaRebuda, uint tsAltresValors);
        public DelPeticioCalcul PeticioCalcul;

        public GestorFuncions(ILogger<GestorFuncions> logger, IOptions<GestorFuncionsOptions> options)
        {
            this._logger = logger;
            //Read the configured functions            
            //and associate them by their parameters
            foreach (var functionOptions in options.Value.Functions)
            {
                if (!_funcionsPerVariable.ContainsKey(functionOptions.ReturnVariable))
                {
                    //Create the script class
                    ParametreScript[] parameters = new ParametreScript[functionOptions.Parameters.Count()];

                    int index = 0;
                    foreach (var param in functionOptions.Parameters)
                    {
                        parameters[index] = new ParametreScript(param.Name, param.AssociatedVariable);
                        index++;
                    }

                    FuncioVariable funcio = new FuncioVariable(functionOptions.ReturnVariable, functionOptions.Function, parameters);

                    _funcionsPerVariable.Add(funcio.NomVariable, funcio);

                    foreach (var parameter in parameters)
                    {
                        if (!_funcionsPerParametres.ContainsKey(parameter.VariableAssociada))
                        {
                            _funcionsPerParametres.Add(parameter.VariableAssociada, new List<FuncioVariable>());
                        }

                        _funcionsPerParametres[parameter.VariableAssociada].Add(funcio);
                    }
                }
                else
                {
                    logger.LogError($"Function definition for variable {functionOptions.ReturnVariable} repeated. Ignored.");
                }
            }
        }    

        public IEnumerable<string> GetNomsVariables()
        {
            return _variables.GetNomsVariables();
        }


        public void RebutDada(string nomVariable,double valor)
        {
            var ts = Interlocked.Increment(ref _timestamp);
            if (ts==0)
            {
                //Overflow
            }

            var dada = _variables.AfegirDada(nomVariable, ts, valor);

            if (dada!=null)
            {
                CalculaRelacionats(nomVariable, dada);
            }
        }

        public void RebutDadaCalculada(string nomVariable, double valor, uint timestamp)
        {            
            var dada = _variables.AfegirDada(nomVariable, timestamp, valor);

            CalculaRelacionats(nomVariable, dada);
        }

        public Dada? DemanaUltimaDada(string nomVariable)
        {
            return _variables.GetUltimaDada(nomVariable);
        }

        private void CalculaRelacionats(string nomVariable, Dada dada)
        {
            if (_funcionsPerParametres.TryGetValue(nomVariable, out var funcions))
            {
                Parallel.ForEach(funcions, funcio =>
                {
                    //Demana calcul
                    //Envia el nom de la variable a calcular
                    //la dada rebuda
                    //el major ts dels paràmetres excepte la rebuda            
                    uint tsAltres = 0;
                    if (funcio.Parametres.Count()>1)
                    {
                        var parametreRebut = funcio.Parametres.Where(p => p.VariableAssociada == nomVariable);
                        tsAltres = funcio.Parametres.Except(parametreRebut).Max(parametre => _variables.GetUltimTimestamp(parametre.VariableAssociada));
                    }
                    


                    if (PeticioCalcul != null)
                        PeticioCalcul.Invoke(funcio.NomVariable, nomVariable, dada, tsAltres);
                    else _logger.LogWarning("PeticioCalcul method undefined");
                });
            }
        }

        
    }
}