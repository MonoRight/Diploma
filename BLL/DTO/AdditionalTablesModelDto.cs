using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class AdditionalTablesModelDto
    {
        //Table1 загальні відомості
        public string T1RegNumDoc { get; set; }
        public string T1RegDate { get; set; }
        public string T1CalibrateDate { get; set; }
        public string T1RezType { get; set; }
        public string T1RezNumber { get; set; }
        public string T1Temperature { get; set; }
        public string T1AtmPressure { get; set; }
        public string T1MethodPoznaka { get; set; }
        public string T1MethodName { get; set; }
        public string T1MethodOrganization { get; set; }

        //Table1.1 Еталонний прилад
        public string T1_1Name { get; set; }
        public string T1_1Type { get; set; }
        public string T1_1ServiceNum { get; set; }
        public string T1_1NumSvidocDovidka { get; set; }
        public string T1_1CalibrateDate { get; set; }
        public string T1_1MainParameters { get; set; }

        //Table1.2 Допоміжне обладнання
        public List<AuxiliaryEquipmentModelDto> T1_2DopomijneObladnannya { get; set; } = new List<AuxiliaryEquipmentModelDto>();

        //Table2.1 Координати точки вимірювання рівня та базова висота резервуара й рівнеміра
        public string T2_1Xb { get; set; }
        public string T2_1Yb { get; set; }
        public string T2_1BaseHeightRez { get; set; }
        public string T2_1BaseHeightRivnemera { get; set; }


        //Table2.2 Параметри рідини під час вимірювань та зберігання
        public string T2_2Name { get; set; }
        public string T2_2Gustina { get; set; }
        public string T2_2Riven { get; set; }
        public string T2_2Tisk { get; set; }
        public string T2_2SeverdnyaGustina { get; set; }


        //Table2.3 Абсолютна висота неконтрольованої порожнини, низу(верху) 
        //зливного (всмоктувального) патрубка, гранична(максимальна) абсолютна висота й температура стінки резервуара
        public string T2_3NekontrPorojnini { get; set; }
        public string T2_3NizyVerhy { get; set; }
        public string T2_3Granichna { get; set; }
        public string T2_3Temperature { get; set; }


        //Table2.4 Параметри стінок резервуара
        public string T2_4CilindrTovshinaStinkiZnachenya { get; set; }
        public string T2_4CilindrTovshinaStinkiGranici { get; set; }
        public string T2_4CilindrTovshinaSharuFarbiZnachenya { get; set; }
        public string T2_4CilindrTovshinaSharuFarbiGranici { get; set; }
        public string T2_4CilindrFormElement { get; set; }

        public string T2_4PerednyeDnisheTovshinaStinkiZnachenya { get; set; }
        public string T2_4PerednyeDnisheTovshinaStinkiGranici { get; set; }
        public string T2_4PerednyeDnisheTovshinaSharuFarbiZnachenya { get; set; }
        public string T2_4PerednyeDnisheTovshinaSharuFarbiGranici { get; set; }
        public string T2_4PerednyeDnisheFormElement { get; set; }

        public string T2_4ZadnyeDnisheTovshinaStinkiZnachenya { get; set; }
        public string T2_4ZadnyeDnisheTovshinaStinkiGranici { get; set; }
        public string T2_4ZadnyeDnisheTovshinaSharuFarbiZnachenya { get; set; }
        public string T2_4ZadnyeDnisheTovshinaSharuFarbiGranici { get; set; }
        public string T2_4ZadnyeDnisheFormElement { get; set; }


        //Table2.5.1 Загальні параметри повірочної рідини згідно з ДСТУ 4218
        public string T2_5_1Name { get; set; }
        public string T2_5_1GustinaRidini { get; set; }
        public string T2_5_1KoeffObem { get; set; }
        public string T2_5_1KoeffStisnennya { get; set; }
        public string T2_5_1KoeffLiniynogo { get; set; }


        //Table2.5.2 Поточні параметри повірочної рідини згідно з ДСТУ 4218
        public string T2_5_2Number { get; set; }
        public string T2_5_2DozovaMist { get; set; }
        public string T2_5_2TemperatureInRez { get; set; }
        public string T2_5_2TemperatureLichilnick { get; set; }
        public string T2_5_2RivenRidini { get; set; }
        public string T2_5_2NadlishkoviyTisk { get; set; }


        //Table2.6 Внутрішні деталі та обладнання резервуара
        public string T2_6Type { get; set; }
        public string T2_6Height { get; set; }
        public string T2_6Lenght { get; set; }
        public string T2_6Diameter { get; set; }
        public string T2_6KutNahily { get; set; }
        public string T2_6Obem { get; set; }
        public string T2_6AbsoluteNijnyaMeja { get; set; }
        public string T2_6AbsoluteVerhnyaMeja { get; set; }


        //Table3.1 Параметри циліндриної частини резервуара
        public string T3_1MiddleRadiusZnachenya { get; set; }
        public string T3_1MiddleRadiusGranici { get; set; }
        public string T3_1ZagalnaDovjinaZnachenya { get; set; }
        public string T3_1ZagalnaDovjinaGranici { get; set; }
        public string T3_1StypinZnachenya { get; set; }
        public string T3_1StypinGranici { get; set; }

        //Table3.2 Відхили внутрішньої поверхні стінки циліндричної частини резервуара від правильної геометричної форми
        public string T3_2KilkistShariv { get; set; }
        public string T3_2KilkistVerticalPeretiniv { get; set; }

        //Table3.3 Загальні параметри резервуара
        public string T3_3MistkistNekontrolovanoiChislove { get; set; }
        public string T3_3MistkistNekontrolovanoiGranici { get; set; }
        public string T3_3MistkistMertvoiChislove { get; set; }
        public string T3_3MistkistMertvoiGranici { get; set; }
        public string T3_3MistkistNaGranichnyVisotyChislove { get; set; }
        public string T3_3MistkistNaGranichnyVisotyGranici { get; set; }


        //GradTable
        public string GradPriznachenya { get; set; }
        public string GradOrganizaciaVlasnik { get; set; }
        public string GradMisceVstanovlenya { get; set; }
        public string GradTypeRez { get; set; }
        public string GradNominalMist { get; set; }
        public string GradGraniciDopustimoiPohibki { get; set; }
        public string GradBasovaVisota { get; set; }
        public string GradGranichnaVisotaNapovnenya { get; set; }
        public string GradMistkistNaGranichnyVisoty { get; set; }
        public string GradDilyankyNizche { get; set; }
        public string GradMistkistMertvoiPoroznini { get; set; }
        public string GradDataProvedenyaPovirki { get; set; }
        public string GradDataChergovoiPovirki { get; set; }
        public string GradVsogoArkushiv { get; set; }
    }
}
