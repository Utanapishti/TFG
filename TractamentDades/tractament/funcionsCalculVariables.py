def TemperaturaPiezometre4500(resistencia):
    return (1/((1.12767e-3+(2.344442e-4*resistencia)+(8.476921e-8*resistencia**3)+(1.175122e-11*resistencia**5))))-273.15

def PressioPiezometre4500(periode,temperatura, pressioAtmosferica):
    G = -0.0005374
    P0 = 8894
    K = 0.03552
    T0 = 22.6
    S0 = 990.1
    pressio = G*(periode-P0)+K*(temperatura-T0)+(pressioAtmosferica-S0)
    return pressio

def PressioPercentual(pressio,nivell):
    return (nivell/pressio)*100
