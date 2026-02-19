using HSM_CommonCS.Constants;

namespace CoolingModel.Models;

public class CoolingConstantsFile
{
    protected double MaxSampleTime = 10.1;        // [s] Maximum sample time
    protected double MinSampleTime = 0.1;   // [s] Minimum sample time

    protected double MinHoldingTime = 0.5;      // [Sec] Minimal holding time
    protected double MaxHoldingTime = 15.0;   // [Sec] Maximal holding time

    protected double MaxWaterTemp = 40.0; // [DegC] Max water temperature
    protected double MinWaterTemp = 3.0;      // [DegC] Min water temperature  //ZZ 211119 was 15
    protected double DefaultWaterTemp = 21.0; // [DegC] Default water temperature

    protected double EntryPyroMinTemp = 575.0; // Intermediate pyro min temperature [DegC] 
    protected double EntryPyroMaxTemp = 1200.0;    // Intermediate pyro max temperature [DegC]
    protected double InterPyroMinTemp = 375.0; // Intermediate pyro min temperature [DegC]
    protected double InterPyroMaxTemp = 1100.0;    // Intermediate pyro max temperature [DegC]
    protected double ExitHighPyroMinTemp = 375.0; // Exit-high range pyro min temperature [DegC] 
    protected double ExitHighPyroMaxTemp = 1100.0;    // Exit-high range pyro max temperature [DegC] 
    protected double ExitLowPyroMinTemp = 70.0;  // Exit-low range pyro min temperature [DegC] 
    protected double ExitLowPyroMaxTemp = 425.0; // Exit-low range pyro max temperature [DegC] 

    protected double TopIntensiveBankMaxFlow = 256.5;   // [m3/h] Top intensive flow at 5 bar pressure (from P&ID diagram) with boost
    protected double BotIntensiveBankMaxFlow = 389.0;    // [m3/h] Bottom intensive flow at 5 bar pressure (from P&ID diagram) with boost
    protected double FlowMeterStatusMinPerUnit = 0.9;        // [pu] Used in autoadatpation for defining an active status of the equipments
    protected double PyroMinReliability = 0.9;        // [pu] Minimal reliability of a temperature sensor

    protected double SHORTTERM_CAMPAIN_ENTEMPVAR = 5.0;     // [%] Maximal variation on entry temperature for defining a production campain 
    protected double SHORTTERM_CAMPAIN_EXTEMPVAR = 30.0;    // [%] Maximal variation on exit temperature for defining a production campain 
    protected double SHORTTERM_CAMPAIN_THK = 5.0;       // [%] Maximal variation on thickness for defining a production campain 

    protected double MIN_HOT_ENDS_LENGTH = 0.5;     // Minimum hot ends (head/tail) length target, below not set => 0.0 - [m]
    protected double MIN_HOT_ENDS_DELTATEMP_TARGET = 5.0;     // [degC] - Minimum hot ends (head/tail) temperature dalta target			//should be 0.0		    
    protected double LATE_STRATEGY_UP_THRESHOLD = 1.8;     // [mm] - Database initialization values: cooling strategy: LATE upper thickness limit threshold  
    protected double EARLY_STRATEG_UP_THRESHOLD = 3.4;      // [mm] - Database initialization values: cooling strategy: EARLY upper thickness limit threshold
    
    protected double DB_INIT_HOT_HEAD_TEMP_DEFAULT = 100.0; // [degC] - Database initialization values: hot head temperature default
    protected double DB_INIT_HOT_TAIL_TEMP_DEFAULT = 0.0;       // [degC] - Database initialization values: hot tail temperature default
    protected double DB_INIT_HOT_HEAD_LENGTH_DEFAULT = 5.0;     // [m] - Database initialization values: hot head length default
    protected double DB_INIT_HOT_TAIL_LENGTH_DEFAULT = 0.0;     // [m] - Database initialization values: hot tail length default
    protected double DB_INIT_UNCOOLED_HEAD_DEFAULT = 0;    // [-] - Database initialization values: uncooled head mode
    protected double DB_INIT_UNCOOLED_HEAD_LEN_DEFAULT = 0.0;       // [m] - Database initialization values: uncooled head length


    protected string engineeringLogFolder = "D:\\Engineering_Logs"; // Engineering logs folder

    protected string COOLING_MODEL_CLUSTER_ID = "COOLING";
    protected string COOLING_MODEL_HTC_EFF_CORR_VAR_ID = "COOL_MODEL_HTC_EFF_CORR";
    protected string COOLING_MODEL_TUNING_VAR_ID = "COOL_MODEL_TUN_PAR";
    protected string COOLING_MODEL_HTC_MODEL_PARS_VAR_ID = "COOL_MODEL_HTC_MODEL_PAR";
    protected string COOLING_MODEL_HTC_ADP_BASIC_VAR_ID = "COOL_MODEL_HTC_ADP_BASIC_PAR";
    protected string COOLING_PARAMETERS_ID = "COOLING_PARAMETERS";
    protected string COOLING_CTRL_TUNING_VAR_ID = "COOLING_CTRL_TUNING_PAR";


    protected double DEFAULT_DC_TARGET_TEMP = 600; // [degC]
    protected double HEAD_FEEDBACK_CONTROL_DELAY=	25; // [m] delay for feedback control from offest
    protected double HEAD_FEEDBACK_CONTROL_TIME_DELAY = 3; // [s] Minimum delay for feedback control in seconds from head
    protected double FEEDBACK_CONTROL_ERROR_MIN = 8; // [s] delay for feedback control in seconds from head

    protected double COOLING_MODEL_ADP_RESET_THCK_THRESH = 0.15; // [-] - Percentage of thickness to be different for short term adaptation reset 25012022
    protected double COOLING_MODEL_ADP_RESET_TRG_DC_TEMP_THRESH = 0.04; // [-] - Percentage of downcoiler temperature to be different for short term adaptation reset 09032022
    protected int SLOW_DOWN_TYPE_FOR_HTC_ADP = (int)SlowDownTypes.SLOW_DOWN_TYPE_HEAD;

}
