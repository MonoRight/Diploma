using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BachelorDiploma.Model
{
    public static class AdditionalTablesModel
    {
        //Table1 загальні відомості
        public static string T1RegNumDoc { get; set; }
        public static string T1RegDate { get; set; }
        public static string T1CalibrateDate { get; set; }
        public static string T1RezType { get; set; }
        public static string T1RezNumber { get; set; }
        public static string T1Temperature { get; set; }
        public static string T1AtmPressure { get; set; }
        public static string T1MethodPoznaka { get; set; }
        public static string T1MethodName { get; set; }
        public static string T1MethodOrganization { get; set; }

        //Table1.1 Еталонний прилад
        public static string T1_1Name { get; set; }
        public static string T1_1Type { get; set; }
        public static string T1_1ServiceNum { get; set; }
        public static string T1_1NumSvidocDovidka { get; set; }
        public static string T1_1CalibrateDate { get; set; }
        public static string T1_1MainParameters { get; set; }

        //Table1.2 Допоміжне обладнання
        public static BindingList<AuxiliaryEquipmentModel> T1_2DopomijneObladnannya { get; set; } = new BindingList<AuxiliaryEquipmentModel>();

        //Table2.1 Координати точки вимірювання рівня та базова висота резервуара й рівнеміра
        public static string T2_1Xb { get; set; }
        public static string T2_1Yb { get; set; }
        public static string T2_1BaseHeightRez { get; set; }
        public static string T2_1BaseHeightRivnemera { get; set; }


        //Table2.2 Параметри рідини під час вимірювань та зберігання
        public static string T2_2Name { get; set; }
        public static string T2_2Gustina { get; set; }
        public static string T2_2Riven { get; set; }
        public static string T2_2Tisk { get; set; }
        public static string T2_2SeverdnyaGustina { get; set; }


        //Table2.3 Абсолютна висота неконтрольованої порожнини, низу(верху) 
        //зливного (всмоктувального) патрубка, гранична(максимальна) абсолютна висота й температура стінки резервуара
        public static string T2_3NekontrPorojnini { get; set; }
        public static string T2_3NizyVerhy { get; set; }
        public static string T2_3Granichna { get; set; }
        public static string T2_3Temperature { get; set; }


        //Table2.4 Параметри стінок резервуара
        public static string T2_4CilindrTovshinaStinkiZnachenya { get; set; }
        public static string T2_4CilindrTovshinaStinkiGranici { get; set; }
        public static string T2_4CilindrTovshinaSharuFarbiZnachenya { get; set; }
        public static string T2_4CilindrTovshinaSharuFarbiGranici { get; set; }
        public static string T2_4CilindrFormElement { get; set; }

        public static string T2_4PerednyeDnisheTovshinaStinkiZnachenya { get; set; }
        public static string T2_4PerednyeDnisheTovshinaStinkiGranici { get; set; }
        public static string T2_4PerednyeDnisheTovshinaSharuFarbiZnachenya { get; set; }
        public static string T2_4PerednyeDnisheTovshinaSharuFarbiGranici { get; set; }
        public static string T2_4PerednyeDnisheFormElement { get; set; }

        public static string T2_4ZadnyeDnisheTovshinaStinkiZnachenya { get; set; }
        public static string T2_4ZadnyeDnisheTovshinaStinkiGranici { get; set; }
        public static string T2_4ZadnyeDnisheTovshinaSharuFarbiZnachenya { get; set; }
        public static string T2_4ZadnyeDnisheTovshinaSharuFarbiGranici { get; set; }
        public static string T2_4ZadnyeDnisheFormElement { get; set; }


        //Table2.5.1 Загальні параметри повірочної рідини згідно з ДСТУ 4218
        public static string T2_5_1Name { get; set; }
        public static string T2_5_1GustinaRidini { get; set; }
        public static string T2_5_1KoeffObem { get; set; }
        public static string T2_5_1KoeffStisnennya { get; set; }
        public static string T2_5_1KoeffLiniynogo { get; set; }


        //Table2.5.2 Поточні параметри повірочної рідини згідно з ДСТУ 4218
        public static string T2_5_2Number { get; set; }
        public static string T2_5_2DozovaMist { get; set; }
        public static string T2_5_2TemperatureInRez { get; set; }
        public static string T2_5_2TemperatureLichilnick { get; set; }
        public static string T2_5_2RivenRidini { get; set; }
        public static string T2_5_2NadlishkoviyTisk { get; set; }


        //Table2.6 Внутрішні деталі та обладнання резервуара
        public static string T2_6Type { get; set; }
        public static string T2_6Height { get; set; }
        public static string T2_6Lenght { get; set; }
        public static string T2_6Diameter { get; set; }
        public static string T2_6KutNahily { get; set; }
        public static string T2_6Obem { get; set; }
        public static string T2_6AbsoluteNijnyaMeja { get; set; }
        public static string T2_6AbsoluteVerhnyaMeja { get; set; }


        //Table3.1 Параметри циліндриної частини резервуара
        public static string T3_1MiddleRadiusZnachenya { get; set; }
        public static string T3_1MiddleRadiusGranici { get; set; }
        public static string T3_1ZagalnaDovjinaZnachenya { get; set; }
        public static string T3_1ZagalnaDovjinaGranici { get; set; }
        public static string T3_1StypinZnachenya { get; set; }
        public static string T3_1StypinGranici { get; set; }

        //Table3.3 Загальні параметри резервуара
        public static string T3_3MistkistNekontrolovanoiChislove { get; set; }
        public static string T3_3MistkistNekontrolovanoiGranici { get; set; }
        public static string T3_3MistkistMertvoiChislove { get; set; }
        public static string T3_3MistkistMertvoiGranici { get; set; }
        public static string T3_3MistkistNaGranichnyVisotyChislove { get; set; }
        public static string T3_3MistkistNaGranichnyVisotyGranici { get; set; }


        //GradTable
        public static string GradPriznachenya { get; set; }
        public static string GradOrganizaciaVlasnik { get; set; }
        public static string GradMisceVstanovlenya { get; set; }
        public static string GradTypeRez { get; set; }
        public static string GradNominalMist { get; set; }
        public static string GradGraniciDopustimoiPohibki { get; set; }
        public static string GradBasovaVisota { get; set; }
        public static string GradGranichnaVisotaNapovnenya { get; set; }
        public static string GradMistkistNaGranichnyVisoty { get; set; }
        public static string GradDilyankyNizche { get; set; }
        public static string GradMistkistMertvoiPoroznini { get; set; }
        public static string GradDataProvedenyaPovirki { get; set; }
        public static string GradDataChergovoiPovirki { get; set; }
        public static string GradVsogoArkushiv { get; set; }
    }
}
