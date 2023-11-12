using BLL.DTO;
using BLL.Enums;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;

namespace BLL
{
    public class ExcelTableService
    {
        private readonly InformationModelDto _informationModel;
        private readonly AdditionalTablesModelDto _additionalTablesModelDto;
        private readonly CalculationResult _calculationResult;

        public ExcelTableService(InformationModelDto informationModel, AdditionalTablesModelDto additionalTablesModelDto, CalculationResult calculationResult)
        {
            _informationModel = informationModel;
            _additionalTablesModelDto = additionalTablesModelDto;
            _calculationResult = calculationResult;
        }

        public void FillExcelFile()
        {
            Application xlApp = new Application();
            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlApp.ActiveSheet.Name = "Градуювальна таблиця";
            double radius = GetRadius(_calculationResult.CentralPereriz, _calculationResult.CentroidOfCentralPereriz);
            double circleSquare = Math.PI * radius * radius;
            double cilindrVolume = circleSquare * (_informationModel.FillingHeight + _informationModel.ZeroPosition);
            double firstVolumeCilindr = cilindrVolume / ((_informationModel.FillingHeight + _informationModel.ZeroPosition) * 100);

            if (xlApp != null)
            {
                int rawCount = _calculationResult.VolumesBetweenHulls.Count / 100 + 2;
                int columnCount = 12;
                //GradTable1
                {
                    int i = 0;
                    int j = 0;
                    int centimeters = 0, temp = 0;
                    List<double> minVolumes = VolumesPerMillimetersArray();

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");

                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.get_Range("a1", "l1").Merge(false);
                    formatRange.HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.Cells[1, 1] = "Градуювальна таблиця (поміліметрові інтервали, приведені до 15°С)";
                    xlWorkSheet.get_Range("a2", "l2").Merge(false);
                    xlWorkSheet.Cells[2, 1] = "Організація " + _additionalTablesModelDto.T1MethodOrganization;
                    xlWorkSheet.get_Range("a3", "f3").Merge(false);
                    xlWorkSheet.Cells[3, 1] = "Тип: " + _additionalTablesModelDto.T1RezType;
                    xlWorkSheet.get_Range("g3", "l3").Merge(false);
                    xlWorkSheet.Cells[3, 7] = "Резервуар № " + _additionalTablesModelDto.T1RezNumber;
                    xlWorkSheet.get_Range("b4", "k4").Merge(false);
                    formatRange.HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.Cells[4, 2] = "Місткість, м3";
                    xlWorkSheet.get_Range("a4", "a6").Merge(false);
                    formatRange.HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.Cells[4, 1] = "Рівень\nнаповнення, см";
                    xlWorkSheet.get_Range("l4", "l6").Merge(false);
                    formatRange.HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.Cells[4, 12] = "Відсоток\nмісткості %";
                    xlWorkSheet.get_Range("b5", "k5").Merge(false);
                    formatRange.HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.Cells[5, 2] = "см/10";

                    for (i = 2; i <= 11; i++)
                    {
                        xlWorkSheet.Cells[6, i] = j;
                        j++;
                    }

                    if (rawCount > 2)
                    {
                        int cols = rawCount + 5;
                        formatRange = xlWorkSheet.get_Range("a4", "l" + cols);
                        formatRange.BorderAround(XlLineStyle.xlContinuous,
                        XlBorderWeight.xlMedium, XlColorIndex.xlColorIndexAutomatic,
                        XlColorIndex.xlColorIndexAutomatic);
                    }
                    else
                    {
                        formatRange = xlWorkSheet.get_Range("a4", "l6");
                        formatRange.BorderAround(XlLineStyle.xlContinuous,
                        XlBorderWeight.xlMedium, XlColorIndex.xlColorIndexAutomatic,
                        XlColorIndex.xlColorIndexAutomatic);
                    }
                    formatRange = xlWorkSheet.get_Range("a4", "l" + rawCount + 4);
                    formatRange.HorizontalAlignment = Constants.xlCenter;


                    for (i = 1; i <= rawCount - 1; i++)
                    {
                        for (j = 0; j <= columnCount - 1; j++)
                        {
                            if (j == 0)
                            {
                                xlWorkSheet.Cells[i + 6, j + 1] = centimeters;
                                continue;
                            }
                            else if (j == 11)
                            {
                                try
                                {
                                    xlWorkSheet.Cells[i + 6, j + 1] = Percentage(minVolumes[temp]);
                                }
                                catch { }
                            }
                            else
                            {
                                if (temp == minVolumes.Count)
                                {
                                    break;
                                }
                                else
                                {
                                    xlWorkSheet.Cells[i + 6, j + 1] = minVolumes[temp];
                                    temp++;
                                }
                            }
                        }
                        centimeters++;
                    }

                    formatRange = xlWorkSheet.get_Range("a4", "l" + rawCount + 4);
                    formatRange.HorizontalAlignment = Constants.xlCenter;
                    formatRange.VerticalAlignment = XlHAlign.xlHAlignCenter;
                    formatRange = xlWorkSheet.get_Range("a1", "l1");
                    xlWorkSheet.Columns.AutoFit();
                }

                //GradTable2
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Град.табл.";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    formatRange = xlWorkSheet.get_Range("d1");
                    formatRange.Font.Underline = true;
                    xlWorkSheet.Cells[2, 2] = "ПОГОДЖЕНО";
                    xlWorkSheet.Cells[2, 8] = "ЗАТВЕРДЖЕНО";
                    xlWorkSheet.Cells[3, 2] = "Начальник лабораторії";
                    xlWorkSheet.Cells[3, 8] = "Директор";
                    xlWorkSheet.Cells[5, 1] = "\"_______\"______________20___р.";
                    xlWorkSheet.Cells[5, 7] = "\"_______\"______________20___р.";
                    formatRange = xlWorkSheet.get_Range("a7");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[7, 4] = "ГРАДУЮВАЛЬНА ТАБЛИЦЯ";
                    xlWorkSheet.get_Range("a9", "i9").Merge(false);
                    xlWorkSheet.get_Range("a9", "i9").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a9", "i9").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a9", "i9");
                    xlWorkSheet.Cells[9, 1] = _additionalTablesModelDto.T1RezNumber;

                    xlWorkSheet.get_Range("e10").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("e10").VerticalAlignment = Constants.xlTop;
                    xlWorkSheet.get_Range("e10").Font.Size = "8";
                    xlWorkSheet.Cells[10, 5] = "(назва, номер резервуара)";

                    xlWorkSheet.get_Range("d11", "i24").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a11", "i24").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a11", "i24");
                    xlWorkSheet.get_Range("a11", "c11").Merge(false);
                    xlWorkSheet.get_Range("d11", "i11").Merge(false);
                    xlWorkSheet.get_Range("a12", "c12").Merge(false);
                    xlWorkSheet.get_Range("d12", "i12").Merge(false);
                    xlWorkSheet.get_Range("a13", "c13").Merge(false);
                    xlWorkSheet.get_Range("d13", "i13").Merge(false);
                    xlWorkSheet.get_Range("a14", "c14").Merge(false);
                    xlWorkSheet.get_Range("d14", "i14").Merge(false);
                    xlWorkSheet.get_Range("a15", "c15").Merge(false);
                    xlWorkSheet.get_Range("d15", "i15").Merge(false);
                    xlWorkSheet.get_Range("a16", "c16").Merge(false);
                    xlWorkSheet.get_Range("d16", "i16").Merge(false);
                    xlWorkSheet.get_Range("a17", "c17").Merge(false);
                    xlWorkSheet.get_Range("d17", "i17").Merge(false);
                    xlWorkSheet.get_Range("a18", "c18").Merge(false);
                    xlWorkSheet.get_Range("d18", "i18").Merge(false);
                    xlWorkSheet.get_Range("a19", "c19").Merge(false);
                    xlWorkSheet.get_Range("d19", "i19").Merge(false);
                    xlWorkSheet.get_Range("a20", "c20").Merge(false);
                    xlWorkSheet.get_Range("d20", "i20").Merge(false);
                    xlWorkSheet.get_Range("a21", "c21").Merge(false);
                    xlWorkSheet.get_Range("d21", "i21").Merge(false);
                    xlWorkSheet.get_Range("a22", "c22").Merge(false);
                    xlWorkSheet.get_Range("d22", "i22").Merge(false);
                    xlWorkSheet.get_Range("a23", "c23").Merge(false);
                    xlWorkSheet.get_Range("d23", "i23").Merge(false);
                    xlWorkSheet.get_Range("a24", "c24").Merge(false);
                    xlWorkSheet.get_Range("d24", "i24").Merge(false);
                    WrapText(ref xlWorkSheet, "a11", "i24");
                    xlWorkSheet.Cells[11, 1] = "Призначення";
                    xlWorkSheet.Cells[12, 1] = "Організація-власник";
                    xlWorkSheet.Cells[13, 1] = "Місце встановлення резервуара (місце виконання вимірювань)";
                    xlWorkSheet.Cells[14, 1] = "Тип резервуара";
                    xlWorkSheet.Cells[15, 1] = "Номінальна міскість";
                    xlWorkSheet.Cells[16, 1] = "Границі допустимої відносної похибки (невизначеність) визначення загальної місткості";
                    xlWorkSheet.Cells[17, 1] = "Базова висота резервуара";
                    xlWorkSheet.Cells[18, 1] = "Гранична абсолютна висота наповнення";
                    xlWorkSheet.Cells[19, 1] = "Місткість на граничну абсолютну висоту наповнення";
                    xlWorkSheet.Cells[20, 1] = "Ділянку нижче ________мм для облікових і торгових операцій не використовують";
                    xlWorkSheet.Cells[21, 1] = "Місткість \"мертвої\" порожнини";
                    xlWorkSheet.Cells[22, 1] = "Дата проведення повірки (калібрування)";
                    xlWorkSheet.Cells[23, 1] = "Дата чергової повірки (калібрування)";
                    xlWorkSheet.Cells[24, 1] = "Всього аркушів у градуювальній таблиці";
                    xlWorkSheet.Cells[11, 4] = _additionalTablesModelDto.GradPriznachenya;
                    xlWorkSheet.Cells[12, 4] = _additionalTablesModelDto.GradOrganizaciaVlasnik;
                    xlWorkSheet.Cells[13, 4] = _additionalTablesModelDto.GradMisceVstanovlenya;
                    xlWorkSheet.Cells[14, 4] = _additionalTablesModelDto.GradTypeRez;
                    xlWorkSheet.Cells[15, 4] = _additionalTablesModelDto.GradNominalMist;
                    xlWorkSheet.Cells[16, 4] = _additionalTablesModelDto.GradGraniciDopustimoiPohibki;
                    xlWorkSheet.Cells[17, 4] = _additionalTablesModelDto.GradBasovaVisota;
                    xlWorkSheet.Cells[18, 4] = _additionalTablesModelDto.GradGranichnaVisotaNapovnenya;
                    xlWorkSheet.Cells[19, 4] = _additionalTablesModelDto.GradMistkistNaGranichnyVisoty;
                    xlWorkSheet.Cells[20, 4] = _additionalTablesModelDto.GradDilyankyNizche;
                    xlWorkSheet.Cells[21, 4] = _additionalTablesModelDto.GradMistkistMertvoiPoroznini;
                    xlWorkSheet.Cells[22, 4] = _additionalTablesModelDto.GradDataProvedenyaPovirki;
                    xlWorkSheet.Cells[23, 4] = _additionalTablesModelDto.GradDataChergovoiPovirki;
                    xlWorkSheet.Cells[24, 4] = _additionalTablesModelDto.GradVsogoArkushiv;
                    xlWorkSheet.Cells[26, 1] = "Організація " + _additionalTablesModelDto.T1MethodOrganization;
                    xlWorkSheet.Cells[27, 1] = "Резервуар № " + _additionalTablesModelDto.T1RezNumber;
                    xlWorkSheet.Cells[28, 1] = "Тип: " + _additionalTablesModelDto.T1RezType;

                    int countVol = (int)(rawCount + 29);
                    int level = 1, row = 44;
                    xlWorkSheet.get_Range("a30", "c" + countVol).HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a30", "c" + countVol).VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a30", "c" + countVol);
                    WrapText(ref xlWorkSheet, "a30", "c" + countVol);
                    xlWorkSheet.Cells[30, 1] = "Рівень наповнення, см";
                    xlWorkSheet.Cells[30, 2] = "Місткість, м³";
                    xlWorkSheet.Cells[30, 3] = "Коєфіціент місткості, м³/см";
                    //xlWorkSheet.Cells[30, 4] = "Похибка(невизнач.) місткості,%";
                    try
                    {
                        for (int i = 1, k = 0, j = 0; i <= rawCount - 1; i++, k++, j++)
                        {
                            xlWorkSheet.Cells[i + 30, 1] = k;

                            if (i == rawCount - 1)
                            {
                                xlWorkSheet.Cells[i + 30, 2] = _calculationResult.Volume;

                                if (_informationModel.TankType == TankType.Vertical)
                                {
                                    xlWorkSheet.Cells[i + 30, 3] = Math.Abs((firstVolumeCilindr * level - _calculationResult.Volume) / 45);
                                }
                                else//для горизонтального
                                {
                                    //xlWorkSheet.Cells[i + 30, 3] = Math.Abs((listOfVolumesHorizontalCylindrPerSantimeter.Last() - _calculationResult.Volume) / 45);
                                }
                            }
                            else
                            {
                                //xlWorkSheet.Cells[i + 30, 2] = dataGridView1[10, i].Value;

                                if (_informationModel.TankType == TankType.Vertical)
                                {
                                    //xlWorkSheet.Cells[i + 30, 3] = Math.Abs((firstVolumeCilindr * level - Convert.ToDouble(dataGridView1[10, i].Value)) / 45);
                                }
                                else//для горизонтального
                                {
                                    //xlWorkSheet.Cells[i + 30, 3] = Math.Abs((listOfVolumesHorizontalCylindrPerSantimeter[j] - Convert.ToDouble(dataGridView1[10, i].Value)) / 45);
                                }
                            }
                            level++;
                            row = i + 30;
                        }
                    }
                    catch { }
                    xlWorkSheet.Cells[row + 4, 1] = "Повірник_____________________________________/________________________________/";
                    row = row + 5;
                    xlWorkSheet.get_Range("a" + row).EntireRow.Font.Size = "8";
                    xlWorkSheet.Cells[row, 3] = "(підпис і відбиток тавра)";
                    xlWorkSheet.Cells[row, 6] = "(ім´я, прізвище)";


                    xlWorkSheet.get_Range("a2").RowHeight = "33";
                    xlWorkSheet.get_Range("a4").RowHeight = "25.20";
                    xlWorkSheet.get_Range("a10").RowHeight = "21.60";
                    xlWorkSheet.get_Range("a11").RowHeight = "19.20";
                    xlWorkSheet.get_Range("a12").RowHeight = "24";
                    xlWorkSheet.get_Range("a13").RowHeight = "34.20";
                    xlWorkSheet.get_Range("a14").RowHeight = "18";
                    xlWorkSheet.get_Range("a15").RowHeight = "18.60";
                    xlWorkSheet.get_Range("a16").RowHeight = "43.20";
                    xlWorkSheet.get_Range("a17").RowHeight = "25.20";
                    xlWorkSheet.get_Range("a18").RowHeight = "31.80";
                    xlWorkSheet.get_Range("a19").RowHeight = "34.80";
                    xlWorkSheet.get_Range("a20").RowHeight = "53";
                    xlWorkSheet.get_Range("a21").RowHeight = "22.80";
                    xlWorkSheet.get_Range("a22").RowHeight = "28.80";
                    xlWorkSheet.get_Range("a23").RowHeight = "31.20";
                    xlWorkSheet.get_Range("a24").RowHeight = "30";
                    xlWorkSheet.get_Range("a30").RowHeight = "64.80";
                    xlWorkSheet.get_Range("a1").ColumnWidth = "10.56";
                    xlWorkSheet.get_Range("b1").ColumnWidth = "8.22";
                    xlWorkSheet.get_Range("c1").ColumnWidth = "11.67";
                    xlWorkSheet.get_Range("d1").ColumnWidth = "9.22";
                    xlWorkSheet.get_Range("e1").ColumnWidth = "8";
                    xlWorkSheet.get_Range("f1").ColumnWidth = "10";
                    xlWorkSheet.get_Range("g1").ColumnWidth = "8";
                    xlWorkSheet.get_Range("h1").ColumnWidth = "8";
                    xlWorkSheet.get_Range("i1").ColumnWidth = "10.33";
                }

                //Table3.3
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл3.3";

                   Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[1, 1] = "Таблиця 3.3 Загальні параметри резервуара";

                    xlWorkSheet.get_Range("a3", "c6").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a3", "c6").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a3", "c6");
                    WrapText(ref xlWorkSheet, "a3", "c6");
                    xlWorkSheet.Cells[3, 1] = "Назва параметра";
                    xlWorkSheet.Cells[3, 2] = "Числове значення м³";
                    xlWorkSheet.Cells[3, 3] = "Границі похибки (розширена невизначеність)";
                    xlWorkSheet.Cells[4, 1] = "Місткість неконтрольованої порожнини";
                    xlWorkSheet.Cells[5, 1] = "Місткість \"мертвої\" порожнини ";
                    xlWorkSheet.Cells[6, 1] = "Місткість на граничну висоту заповнення резервуара (загальна місткість)";
                    xlWorkSheet.Cells[4, 2] = _calculationResult.MistkistNekontrolovanoi + " м³";
                    xlWorkSheet.Cells[4, 3] = "± " + _calculationResult.MistkistNekontrolovanoiGraniciPohibki + " м³";
                    xlWorkSheet.Cells[5, 2] = _calculationResult.MistkistMertvoi + " м³";
                    xlWorkSheet.Cells[5, 3] = "± " + _calculationResult.MistkistMertvoiGraniciPohibki + " м³";
                    xlWorkSheet.Cells[6, 2] = _calculationResult.FullVolume + " м³";
                    xlWorkSheet.Cells[6, 3] = "± " + _calculationResult.FullVolumeGraniciPohibki + " м³";
                    xlWorkSheet.get_Range("a3").RowHeight = "43.80";
                    xlWorkSheet.get_Range("a6").RowHeight = "34.20";
                    xlWorkSheet.get_Range("a1").ColumnWidth = "39.44";
                    xlWorkSheet.get_Range("b1").ColumnWidth = "13.56";
                    xlWorkSheet.get_Range("c1").ColumnWidth = "21.89";
                }


                //Table3.2 VERY BIG!!!!
                if (_informationModel.TankType == TankType.Vertical)
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл3.2";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[1, 1] = "Таблиця 3.2 Відхили внутрішньої поверхні стінки циліндричної частини резервуара від правильної геометричної форми";
                    if (_calculationResult.VidhilenyaPoPererizam.Count > 0)
                    {
                        xlWorkSheet.get_Range("a3", "a5").Merge(false);
                        xlWorkSheet.get_Range("a3").RowHeight = "20.40";
                        xlWorkSheet.get_Range("a4").RowHeight = "18.00";
                        xlWorkSheet.get_Range("a5").RowHeight = "19.20";
                        xlWorkSheet.get_Range("a1").ColumnWidth = "11.00";

                        double standartnaNeviznachennist = 0, seredne = 0, count = 0;

                        foreach (var pererizi in _calculationResult.VidhilenyaPoPererizam)
                        {
                            foreach (var hordi in pererizi)
                            {
                                count++;
                                seredne += hordi;
                            }
                        }

                        seredne = seredne / count;

                        foreach (var pererizi in _calculationResult.VidhilenyaPoPererizam)
                        {
                            foreach (var hordi in pererizi)
                            {
                                standartnaNeviznachennist += Math.Pow(hordi - seredne, 2);
                            }
                        }
                        standartnaNeviznachennist = Math.Sqrt(standartnaNeviznachennist / (count * (count - 1)));

                        int countOfHords = 0;
                        foreach (var p in _calculationResult.VidhilenyaPoPererizam)
                        {
                            countOfHords = p.Count;
                            break;
                        }
                        countOfHords += 1;

                        xlWorkSheet.Range[xlWorkSheet.Cells[3, 2], xlWorkSheet.Cells[3, countOfHords]].Merge(false);
                        xlWorkSheet.Range[xlWorkSheet.Cells[4, 2], xlWorkSheet.Cells[4, countOfHords]].Merge(false);
                        xlWorkSheet.Range[xlWorkSheet.Cells[3, 2], xlWorkSheet.Cells[3, countOfHords]].HorizontalAlignment = Constants.xlCenter;
                        xlWorkSheet.Range[xlWorkSheet.Cells[4, 2], xlWorkSheet.Cells[4, countOfHords]].HorizontalAlignment = Constants.xlCenter;
                        xlWorkSheet.Range[xlWorkSheet.Cells[3, 2], xlWorkSheet.Cells[3, countOfHords]].VerticalAlignment = Constants.xlCenter;
                        xlWorkSheet.Range[xlWorkSheet.Cells[4, 2], xlWorkSheet.Cells[4, countOfHords]].VerticalAlignment = Constants.xlCenter;
                        xlWorkSheet.Cells[3, 1] = "Номер поперечного перерізу";
                        xlWorkSheet.Cells[3, 1].WrapText = true;
                        xlWorkSheet.Cells[3, 2] = "Відхили внутрішньої поверхні стінки циліндричної частини резервуара від правильної геометричної форми, мм";
                        xlWorkSheet.Cells[4, 2] = "Номери поздовжніх перерізів";
                        xlWorkSheet.Range[xlWorkSheet.Cells[3, 1], xlWorkSheet.Cells[5, countOfHords]].Cells.Borders.LineStyle = XlLineStyle.xlContinuous;
                        xlWorkSheet.Columns.HorizontalAlignment = Constants.xlCenter;
                        xlWorkSheet.Columns.VerticalAlignment = Constants.xlCenter;

                        for (int i = 2, k = 1; i <= countOfHords; i++, k++)
                        {
                            xlWorkSheet.Cells[5, i] = k;
                            xlWorkSheet.Cells[5, i].Borders.LineStyle = XlLineStyle.xlContinuous;
                        }

                        int b = 1;
                        int sharNumber = 1, rowNum = 6, colNum = 2;
                        foreach (var pereriz in _calculationResult.VidhilenyaPoPererizam)
                        {
                            if (b == 0)
                            {
                                xlWorkSheet.Cells[rowNum, 1] = sharNumber + " Низ";
                                xlWorkSheet.Cells[rowNum, 1].Borders.LineStyle = XlLineStyle.xlContinuous;

                                foreach (var hord in pereriz)
                                {
                                    xlWorkSheet.Cells[rowNum, colNum] = hord;
                                    xlWorkSheet.Cells[rowNum, colNum].Borders.LineStyle = XlLineStyle.xlContinuous;
                                    colNum++;
                                }
                                colNum = 2;
                                b++;
                            }
                            else if (b == 1)
                            {
                                xlWorkSheet.Cells[rowNum, 1] = sharNumber + " Сер";
                                xlWorkSheet.Cells[rowNum, 1].Borders.LineStyle = XlLineStyle.xlContinuous;

                                foreach (var hord in pereriz)
                                {
                                    xlWorkSheet.Cells[rowNum, colNum] = hord;
                                    xlWorkSheet.Cells[rowNum, colNum].Borders.LineStyle = XlLineStyle.xlContinuous;
                                    colNum++;
                                }
                                colNum = 2;
                                b++;
                            }
                            else
                            {
                                xlWorkSheet.Cells[rowNum, 1] = sharNumber + " Вер";
                                xlWorkSheet.Cells[rowNum, 1].Borders.LineStyle = XlLineStyle.xlContinuous;

                                foreach (var hord in pereriz)
                                {
                                    xlWorkSheet.Cells[rowNum, colNum] = hord;
                                    xlWorkSheet.Cells[rowNum, colNum].Borders.LineStyle = XlLineStyle.xlContinuous;
                                    colNum++;
                                }
                                colNum = 2;
                                sharNumber++;
                                b = 0;
                            }
                            rowNum++;
                        }
                        xlWorkSheet.Cells[rowNum + 3, 1] = "Середній квадратичний радіальний відхил (стандартна невизначеність) реальної поверхні циліндричної частини резервуар від циліндра " + standartnaNeviznachennist + " мм";
                        xlWorkSheet.get_Range("a1").HorizontalAlignment = Constants.xlLeft;
                        xlWorkSheet.Cells[rowNum + 3, 1].HorizontalAlignment = Constants.xlLeft;
                    }
                }

                //Table3.1
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл3.1";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[1, 1] = "Таблиця 3 Результати обчислення";
                    formatRange = xlWorkSheet.get_Range("a2");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[2, 1] = "Таблиця 3.1 Параметри циліндриної частини резервуара";

                    xlWorkSheet.get_Range("a4", "c7").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a4", "c7").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a4", "c7");
                    WrapText(ref xlWorkSheet, "a4", "c7");
                    xlWorkSheet.Cells[4, 1] = "Назва параметра";
                    xlWorkSheet.Cells[5, 1] = "Середній радіус циліндричної частини резервуара, приведений до 15°С";
                    xlWorkSheet.Cells[6, 1] = "Загальна довжина циліндричної частини резервуара";
                    xlWorkSheet.Cells[7, 1] = "Ступінь нахилу осі резервуара";
                    xlWorkSheet.Cells[4, 2] = "Числове значення";
                    xlWorkSheet.Cells[4, 3] = "Границі похибки (розширена невизначеність)";

                    xlWorkSheet.Cells[5, 2] = _calculationResult.SeredniiRadius + " мм";
                    xlWorkSheet.Cells[5, 3] = "± " + _calculationResult.SeredniiRadiusGraniciPohibki + " мм";
                    xlWorkSheet.Cells[6, 2] = _calculationResult.ZagalnaDovjina + " мм";
                    xlWorkSheet.Cells[6, 3] = "± " + _calculationResult.ZagalnaDovjinaGraniciPohibki + " мм";
                    xlWorkSheet.Cells[7, 2] = _calculationResult.CutNahily + " мм/м";
                    xlWorkSheet.Cells[7, 3] = "± " + _calculationResult.CutNahilyGraniciPohibki + " мм/м";
                    xlWorkSheet.get_Range("a4").RowHeight = "43.20";
                    xlWorkSheet.get_Range("a5").RowHeight = "28.80";
                    xlWorkSheet.get_Range("a6").RowHeight = "28.80";
                    xlWorkSheet.get_Range("a1").ColumnWidth = "36.22";
                    xlWorkSheet.get_Range("b1").ColumnWidth = "19.22";
                    xlWorkSheet.get_Range("c1").ColumnWidth = "22.11";
                }

                //Table2.6
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл2.6";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[1, 1] = "Таблиця 2.6 Внутрішні деталі та обладнання резервуара";

                    xlWorkSheet.get_Range("a3", "h5").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a3", "h5").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a3", "h5");
                    WrapText(ref xlWorkSheet, "a3", "h5");
                    xlWorkSheet.get_Range("a3", "a4").Merge(false);
                    xlWorkSheet.get_Range("b3", "b4").Merge(false);
                    xlWorkSheet.get_Range("c3", "c4").Merge(false);
                    xlWorkSheet.get_Range("d3", "d4").Merge(false);
                    xlWorkSheet.get_Range("e3", "e4").Merge(false);
                    xlWorkSheet.get_Range("f3", "f4").Merge(false);
                    xlWorkSheet.get_Range("g3", "h3").Merge(false);
                    xlWorkSheet.Cells[3, 1] = "Тип";
                    xlWorkSheet.Cells[3, 2] = "Висота, мм";
                    xlWorkSheet.Cells[3, 3] = "Довжина, мм";
                    xlWorkSheet.Cells[3, 4] = "Діаметр (ширина), мм";
                    xlWorkSheet.Cells[3, 5] = "Кут нахилу осі, °С";
                    xlWorkSheet.Cells[3, 6] = "Об´єм, м³";
                    xlWorkSheet.Cells[3, 7] = "Абсолютна висота, мм";
                    xlWorkSheet.Cells[4, 7] = "нижня межа";
                    xlWorkSheet.Cells[4, 8] = "верхня межа";
                    xlWorkSheet.Cells[5, 1] = _additionalTablesModelDto.T2_6Type;
                    xlWorkSheet.Cells[5, 2] = _additionalTablesModelDto.T2_6Height;
                    xlWorkSheet.Cells[5, 3] = _additionalTablesModelDto.T2_6Lenght;
                    xlWorkSheet.Cells[5, 4] = _additionalTablesModelDto.T2_6Diameter;
                    xlWorkSheet.Cells[5, 5] = _additionalTablesModelDto.T2_6KutNahily;
                    xlWorkSheet.Cells[5, 6] = _additionalTablesModelDto.T2_6Obem;
                    xlWorkSheet.Cells[5, 7] = _additionalTablesModelDto.T2_6AbsoluteNijnyaMeja;
                    xlWorkSheet.Cells[5, 8] = _additionalTablesModelDto.T2_6AbsoluteVerhnyaMeja;
                    xlWorkSheet.get_Range("a3").RowHeight = "31.80";
                    xlWorkSheet.get_Range("a4").RowHeight = "28.80";
                    xlWorkSheet.get_Range("g1").ColumnWidth = "5.89";
                    xlWorkSheet.get_Range("h1").ColumnWidth = "6.22";
                    xlWorkSheet.get_Range("a3", "f5").Columns.AutoFit();
                    xlWorkSheet.Cells[6, 1] = "Примітка. Довжина деталі \" - \" збільшує місткість резервуара, довжина деталі \" + \" зменшує місткість резервуара.";
                }

                //Table2.5.2
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл2.5.2";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[1, 1] = "Таблиця 2.5.2 Поточні параметри повірочної рідини згідно з ДСТУ 4218";

                    xlWorkSheet.get_Range("a3", "f5").HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    xlWorkSheet.get_Range("a3", "f5").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a3", "f5");
                    WrapText(ref xlWorkSheet, "a3", "f5");
                    xlWorkSheet.get_Range("a3", "a4").Merge(false);
                    xlWorkSheet.get_Range("b3", "b4").Merge(false);
                    xlWorkSheet.get_Range("c3", "d3").Merge(false);
                    xlWorkSheet.get_Range("e3", "e4").Merge(false);
                    xlWorkSheet.get_Range("f3", "f4").Merge(false);
                    xlWorkSheet.Cells[3, 1] = "Номер \nвимірювання";
                    xlWorkSheet.Cells[3, 2] = "Дозова місткість \n(об´єм дози), м³";
                    xlWorkSheet.Cells[3, 3] = "Температура рідини, °С";
                    xlWorkSheet.Cells[3, 5] = "Рівень \nрідини, \nмм";
                    xlWorkSheet.Cells[3, 6] = "Надлишковий тиск у лічильнику рідини, \nМпа";
                    xlWorkSheet.Cells[4, 3] = "у резервуарі";
                    xlWorkSheet.Cells[4, 4] = "у лічильнику \nрідини";
                    xlWorkSheet.Cells[5, 1] = _additionalTablesModelDto.T2_5_2Number;
                    xlWorkSheet.Cells[5, 2] = _additionalTablesModelDto.T2_5_2DozovaMist;
                    xlWorkSheet.Cells[5, 3] = _additionalTablesModelDto.T2_5_2TemperatureInRez;
                    xlWorkSheet.Cells[5, 4] = _additionalTablesModelDto.T2_5_2TemperatureLichilnick;
                    xlWorkSheet.Cells[5, 5] = _additionalTablesModelDto.T2_5_2RivenRidini;
                    xlWorkSheet.Cells[5, 6] = _additionalTablesModelDto.T2_5_2NadlishkoviyTisk;
                    xlWorkSheet.get_Range("a3").RowHeight = "19.20";
                    xlWorkSheet.get_Range("a4").RowHeight = "53.40";
                    xlWorkSheet.get_Range("a1").ColumnWidth = "11.56";
                    xlWorkSheet.get_Range("b1").ColumnWidth = "14.78";
                    xlWorkSheet.get_Range("c1").ColumnWidth = "10.67";
                    xlWorkSheet.get_Range("d1").ColumnWidth = "11.00";
                    xlWorkSheet.get_Range("e1").ColumnWidth = "8.67";
                    xlWorkSheet.get_Range("f1").ColumnWidth = "12.56";
                }

                //Table2.5.1
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл2.5.1";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[1, 1] = "Таблиця 2.5. Параметри повірочної рідини";
                    xlWorkSheet.get_Range("a2", "e2").Font.Bold = true;
                    xlWorkSheet.Cells[2, 1] = "Таблиця 2.5.1  Загальні параметри повірочної рідини згідно з ДСТУ 4218";

                    xlWorkSheet.get_Range("a4", "e5").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a4", "e5").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a4", "e5");
                    WrapText(ref xlWorkSheet, "a4", "e5");
                    xlWorkSheet.Cells[4, 1] = "Назва";
                    xlWorkSheet.Cells[4, 2] = "Густина рідини, що повіряють, кг/м³";
                    xlWorkSheet.Cells[4, 3] = "Коєфіцієнт об'ємного розширення, 10¯³·1/°С";
                    xlWorkSheet.Cells[4, 4] = "Коєфіціент стиснення рідини, 10¯³·1/°С";
                    xlWorkSheet.Cells[4, 5] = "Коєфіціент лінійного розширення резервуара, 10¯³1/°С";
                    xlWorkSheet.Cells[5, 1] = _additionalTablesModelDto.T2_5_1Name;
                    xlWorkSheet.Cells[5, 2] = _additionalTablesModelDto.T2_5_1GustinaRidini;
                    xlWorkSheet.Cells[5, 3] = _additionalTablesModelDto.T2_5_1KoeffObem;
                    xlWorkSheet.Cells[5, 4] = _additionalTablesModelDto.T2_5_1KoeffStisnennya;
                    xlWorkSheet.Cells[5, 5] = _additionalTablesModelDto.T2_5_1KoeffLiniynogo;
                    xlWorkSheet.get_Range("a4").RowHeight = "60.60";
                    xlWorkSheet.get_Range("b1").ColumnWidth = "13.89";
                    xlWorkSheet.get_Range("c1").ColumnWidth = "16.78";
                    xlWorkSheet.get_Range("d1").ColumnWidth = "15.11";
                    xlWorkSheet.get_Range("e1").ColumnWidth = "18.56";
                    xlWorkSheet.get_Range("a4", "a5").Columns.AutoFit();
                }

                //Table2.4
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл2.4";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[1, 1] = "Таблиця 2.4. Параметри стінок резервуара";

                    xlWorkSheet.get_Range("a4", "f8").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a4", "f8").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a4", "f8");
                    WrapText(ref xlWorkSheet, "a4", "f8");
                    xlWorkSheet.get_Range("a4", "a5").Merge(false);
                    xlWorkSheet.get_Range("b4", "c4").Merge(false);
                    xlWorkSheet.get_Range("d4", "e4").Merge(false);
                    xlWorkSheet.get_Range("f4", "f5").Merge(false);
                    xlWorkSheet.Cells[4, 1] = "Елемент резервуара";
                    xlWorkSheet.Cells[4, 2] = "Товщина стінки,мм";
                    xlWorkSheet.Cells[4, 4] = "Товщина шару фарби стінки, мм";
                    xlWorkSheet.Cells[4, 6] = "Форма елемента";
                    xlWorkSheet.Cells[5, 2] = "значення";
                    xlWorkSheet.Cells[5, 3] = "границі похибки";
                    xlWorkSheet.Cells[5, 4] = "значення";
                    xlWorkSheet.Cells[5, 5] = "границі похибки";
                    xlWorkSheet.Cells[6, 1] = "Циліндрична частина";
                    xlWorkSheet.Cells[7, 1] = "Передня днище";
                    xlWorkSheet.Cells[8, 1] = "Заднє днище";
                    xlWorkSheet.Cells[6, 2] = _additionalTablesModelDto.T2_4CilindrTovshinaStinkiZnachenya;
                    xlWorkSheet.Cells[6, 3] = _additionalTablesModelDto.T2_4CilindrTovshinaStinkiGranici;
                    xlWorkSheet.Cells[6, 4] = _additionalTablesModelDto.T2_4CilindrTovshinaSharuFarbiZnachenya;
                    xlWorkSheet.Cells[6, 5] = _additionalTablesModelDto.T2_4CilindrTovshinaSharuFarbiGranici;
                    xlWorkSheet.Cells[6, 6] = _additionalTablesModelDto.T2_4CilindrFormElement;
                    xlWorkSheet.Cells[7, 2] = _additionalTablesModelDto.T2_4PerednyeDnisheTovshinaStinkiZnachenya;
                    xlWorkSheet.Cells[7, 3] = _additionalTablesModelDto.T2_4PerednyeDnisheTovshinaStinkiGranici;
                    xlWorkSheet.Cells[7, 4] = _additionalTablesModelDto.T2_4PerednyeDnisheTovshinaSharuFarbiZnachenya;
                    xlWorkSheet.Cells[7, 5] = _additionalTablesModelDto.T2_4PerednyeDnisheTovshinaSharuFarbiGranici;
                    xlWorkSheet.Cells[7, 6] = _additionalTablesModelDto.T2_4PerednyeDnisheFormElement;
                    xlWorkSheet.Cells[8, 2] = _additionalTablesModelDto.T2_4ZadnyeDnisheTovshinaStinkiZnachenya;
                    xlWorkSheet.Cells[8, 3] = _additionalTablesModelDto.T2_4ZadnyeDnisheTovshinaStinkiGranici;
                    xlWorkSheet.Cells[8, 4] = _additionalTablesModelDto.T2_4ZadnyeDnisheTovshinaSharuFarbiZnachenya;
                    xlWorkSheet.Cells[8, 5] = _additionalTablesModelDto.T2_4ZadnyeDnisheTovshinaSharuFarbiGranici;
                    xlWorkSheet.Cells[8, 6] = _additionalTablesModelDto.T2_4ZadnyeDnisheFormElement;
                    xlWorkSheet.get_Range("a4").RowHeight = "31.80";
                    xlWorkSheet.get_Range("a5").RowHeight = "28.80";
                    xlWorkSheet.get_Range("a1").ColumnWidth = "18.90";
                    xlWorkSheet.get_Range("b1").ColumnWidth = "8.33";
                    xlWorkSheet.get_Range("c1").ColumnWidth = "10.89";
                    xlWorkSheet.get_Range("d1").ColumnWidth = "8.33";
                    xlWorkSheet.get_Range("e1").ColumnWidth = "11.00";
                    xlWorkSheet.get_Range("f1").ColumnWidth = "14.89";
                }

                //Table2.3
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл2.3";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.get_Range("a1", "d1").Merge(false);
                    xlWorkSheet.Cells[1, 1] = "Таблиця 2.3. Абсалютна висота неконтрольованої порожнини, низу (верху) зливного (всмоктувального) патрубка, гранична (максимальна) абсалютна висота й температура стінки резервуара";

                    xlWorkSheet.get_Range("a4", "d6").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a4", "d6").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a4", "d6");
                    WrapText(ref xlWorkSheet, "a1", "d6");
                    xlWorkSheet.get_Range("a4", "c4").Merge(false);
                    xlWorkSheet.get_Range("d4", "d5").Merge(false);
                    xlWorkSheet.Cells[4, 1] = "Абсалютна висота, мм";
                    xlWorkSheet.Cells[4, 4] = "Температура стінки резервуара, °С";
                    xlWorkSheet.Cells[5, 1] = "Неконтрольованої порожнини";
                    xlWorkSheet.Cells[5, 2] = "низу(верху) зливного (всмоктувального) патрубка";
                    xlWorkSheet.Cells[5, 3] = "гранична (максимальна)";
                    xlWorkSheet.Cells[6, 1] = _additionalTablesModelDto.T2_3NekontrPorojnini;
                    xlWorkSheet.Cells[6, 2] = _additionalTablesModelDto.T2_3NizyVerhy;
                    xlWorkSheet.Cells[6, 3] = _additionalTablesModelDto.T2_3Granichna;
                    xlWorkSheet.Cells[6, 4] = _additionalTablesModelDto.T2_3Temperature;
                    xlWorkSheet.get_Range("a1").RowHeight = "45.60";
                    xlWorkSheet.get_Range("a5").RowHeight = "50.40";
                    xlWorkSheet.get_Range("a1").ColumnWidth = "17.00";
                    xlWorkSheet.get_Range("b1").ColumnWidth = "22.44";
                    xlWorkSheet.get_Range("c1").ColumnWidth = "13.11";
                    xlWorkSheet.get_Range("d1").ColumnWidth = "16.11";
                }

                //Table2.2
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл2.2";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[1, 1] = "Таблиця 2.2. Параметри рідини під час вимірювань та зберігання";

                    xlWorkSheet.get_Range("a4", "e8").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a4", "e8").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a4", "e8");
                    WrapText(ref xlWorkSheet, "a4", "e8");
                    xlWorkSheet.get_Range("a4", "d4").Merge(false);
                    xlWorkSheet.get_Range("a5", "a7").Merge(false);
                    xlWorkSheet.get_Range("b5", "b7").Merge(false);
                    xlWorkSheet.get_Range("c5", "c7").Merge(false);
                    xlWorkSheet.get_Range("d5", "d7").Merge(false);
                    xlWorkSheet.get_Range("e4", "e7").Merge(false);
                    xlWorkSheet.Cells[4, 1] = "Параметр рідини, що міститься в резервуарі під час вимірювань";
                    xlWorkSheet.Cells[4, 5] = "Середня густина рідини в резервуарі під час зберігання, кг/м³";
                    xlWorkSheet.Cells[5, 1] = "Назва";
                    xlWorkSheet.Cells[5, 2] = "Густина, кг/м³";
                    xlWorkSheet.Cells[5, 3] = "рівень, мм";
                    xlWorkSheet.Cells[5, 4] = "тиск, Мпа";
                    xlWorkSheet.Cells[8, 1] = _additionalTablesModelDto.T2_2Name;
                    xlWorkSheet.Cells[8, 2] = _additionalTablesModelDto.T2_2Gustina;
                    xlWorkSheet.Cells[8, 3] = _additionalTablesModelDto.T2_2Riven;
                    xlWorkSheet.Cells[8, 4] = _additionalTablesModelDto.T2_2Tisk;
                    xlWorkSheet.Cells[8, 5] = _additionalTablesModelDto.T2_2SeverdnyaGustina;
                    xlWorkSheet.get_Range("a4").RowHeight = "34.20";
                    xlWorkSheet.get_Range("a7").RowHeight = "16.20";
                    xlWorkSheet.get_Range("b1").ColumnWidth = "8.00";
                    xlWorkSheet.get_Range("c1").ColumnWidth = "7.00";
                    xlWorkSheet.get_Range("d1").ColumnWidth = "6.78";
                    xlWorkSheet.get_Range("e1").ColumnWidth = "13.89";
                    xlWorkSheet.get_Range("a5", "a8").Columns.AutoFit();
                }

                //Table2.1
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл2.1";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[1, 1] = "Таблиця 2.1. Координати точки вимірювання рівня та базова висота резервуара й рівнеміра";

                    xlWorkSheet.get_Range("a4", "d6").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a4", "d6").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a4", "d6");
                    xlWorkSheet.get_Range("a4", "a5").Merge(false);
                    xlWorkSheet.Cells[4, 1] = "Хb, м";
                    xlWorkSheet.get_Range("b4", "b5").Merge(false);
                    xlWorkSheet.Cells[4, 2] = "Yb, м";
                    xlWorkSheet.Cells[4, 3] = "Базова висота ";
                    xlWorkSheet.Cells[4, 4] = "Базова висота";
                    xlWorkSheet.Cells[5, 3] = "резервуара, мм";
                    xlWorkSheet.Cells[5, 4] = "рівнеміра, мм";
                    xlWorkSheet.Cells[6, 1] = _additionalTablesModelDto.T2_1Xb;
                    xlWorkSheet.Cells[6, 2] = _additionalTablesModelDto.T2_1Yb;
                    xlWorkSheet.Cells[6, 3] = _additionalTablesModelDto.T2_1BaseHeightRez;
                    xlWorkSheet.Cells[6, 4] = _additionalTablesModelDto.T2_1BaseHeightRivnemera;

                    xlWorkSheet.get_Range("a4", "d6").Columns.AutoFit();

                }

                //Table1.2
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл1.2";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[1, 1] = "Таблиця 1.2. Допоміжне обладнання";

                    int count = _additionalTablesModelDto.T1_2DopomijneObladnannya.Count + 3;
                    xlWorkSheet.get_Range("a3", "d" + count).HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a3", "d" + count).VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a3", "d" + count);
                    xlWorkSheet.Cells[3, 1] = "Назва";
                    xlWorkSheet.Cells[3, 2] = "Тип, виробник";
                    xlWorkSheet.Cells[3, 3] = "Серійний номер";
                    xlWorkSheet.Cells[3, 4] = "Номер свідоцтва про повірку (калібрування)";

                    int rowNum = 4;
                    foreach (AuxiliaryEquipmentModelDto auxiliaryEquipmentModelDto in _additionalTablesModelDto.T1_2DopomijneObladnannya)
                    {
                        xlWorkSheet.Cells[rowNum, 1] = auxiliaryEquipmentModelDto.Name;
                        xlWorkSheet.Cells[rowNum, 2] = auxiliaryEquipmentModelDto.Type;
                        xlWorkSheet.Cells[rowNum, 3] = auxiliaryEquipmentModelDto.SerialNumber;
                        xlWorkSheet.Cells[rowNum, 4] = auxiliaryEquipmentModelDto.SertificateNumber;
                        rowNum++;
                    }
                    xlWorkSheet.get_Range("a3", "d" + count).Columns.AutoFit();
                }

                //Table1.1
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл1.1";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.Cells[1, 1] = "Таблиця 1.1.Еталонний прилад";

                    xlWorkSheet.get_Range("a3", "b8").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a3", "b8").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a3", "b8");
                    xlWorkSheet.Cells[3, 1] = "Назва";
                    xlWorkSheet.Cells[3, 2] = _additionalTablesModelDto.T1_1Name;
                    xlWorkSheet.Cells[4, 1] = "Тип, виробник";
                    xlWorkSheet.Cells[4, 2] = _additionalTablesModelDto.T1_1Type;
                    xlWorkSheet.Cells[5, 1] = "Сервісний номер";
                    xlWorkSheet.Cells[5, 2] = _additionalTablesModelDto.T1_1ServiceNum;
                    xlWorkSheet.Cells[6, 1] = "Номер свідоцтва про повірку";
                    xlWorkSheet.Cells[6, 2] = _additionalTablesModelDto.T1_1NumSvidocDovidka;
                    xlWorkSheet.Cells[7, 1] = "Дата повірка (калібрування)";
                    xlWorkSheet.Cells[7, 2] = _additionalTablesModelDto.T1_1CalibrateDate;
                    xlWorkSheet.Cells[8, 1] = "Основні параметри";
                    xlWorkSheet.Cells[8, 2] = _additionalTablesModelDto.T1_1MainParameters;

                    xlWorkSheet.get_Range("a3", "b8").Columns.AutoFit();
                }

                //Table1
                {
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Add(misValue, misValue, misValue, misValue);
                    xlApp.ActiveSheet.Name = "Табл1";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("a1");
                    formatRange.EntireRow.Font.Bold = true;
                    xlWorkSheet.get_Range("a1", "d1").Merge(false);
                    formatRange.HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.Cells[1, 1] = "Протокол повірки резервуару";

                    xlWorkSheet.get_Range("a3", "d3").Merge(false);
                    xlWorkSheet.get_Range("a3", "d3").Font.Bold = true;
                    xlWorkSheet.Cells[3, 1] = "Таблиця 1 Загальні відомості";


                    xlWorkSheet.get_Range("a5", "d6").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a5", "d6").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a5", "d6");
                    xlWorkSheet.get_Range("a5", "b5").Merge(false);
                    xlWorkSheet.get_Range("a6", "b6").Merge(false);
                    xlWorkSheet.Cells[5, 1] = "Реєстраційний номер документа";
                    xlWorkSheet.Cells[6, 1] = _additionalTablesModelDto.T1RegNumDoc;
                    xlWorkSheet.Cells[5, 3] = "Дата реєстрації";
                    xlWorkSheet.Cells[6, 3] = _additionalTablesModelDto.T1RegDate;
                    xlWorkSheet.Cells[5, 4] = "Дата повірки(калібрування)";
                    xlWorkSheet.Cells[6, 4] = _additionalTablesModelDto.T1CalibrateDate;


                    xlWorkSheet.get_Range("a9", "d10").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a9", "d10").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a9", "d10");
                    xlWorkSheet.Cells[9, 1] = "Тип резервуара";
                    xlWorkSheet.Cells[10, 1] = _additionalTablesModelDto.T1RezType;
                    xlWorkSheet.Cells[9, 2] = "Номер резервуара";
                    xlWorkSheet.Cells[10, 2] = _additionalTablesModelDto.T1RezNumber;
                    xlWorkSheet.Cells[9, 3] = "Температура повітря, °С";
                    xlWorkSheet.Cells[10, 3] = _additionalTablesModelDto.T1Temperature;
                    xlWorkSheet.Cells[9, 4] = "Атмосферний тиск, кПа";
                    xlWorkSheet.Cells[10, 4] = _additionalTablesModelDto.T1AtmPressure;


                    xlWorkSheet.get_Range("a13", "d14").HorizontalAlignment = Constants.xlCenter;
                    xlWorkSheet.get_Range("a13", "d14").VerticalAlignment = XlHAlign.xlHAlignCenter;
                    MakeBorderCells(ref xlWorkSheet, "a13", "d14");
                    xlWorkSheet.Cells[13, 2] = "Познака";
                    xlWorkSheet.Cells[14, 2] = _additionalTablesModelDto.T1MethodPoznaka;
                    xlWorkSheet.Cells[13, 3] = "Назва";
                    xlWorkSheet.Cells[14, 3] = _additionalTablesModelDto.T1MethodName;
                    xlWorkSheet.Cells[13, 4] = "Організація-розробник";
                    xlWorkSheet.Cells[14, 4] = _additionalTablesModelDto.T1MethodOrganization;
                    xlWorkSheet.Cells[14, 1] = "Методика повірки (калібрування)";

                    xlWorkSheet.Columns.AutoFit();
                }

                try
                {
                    xlWorkBook.SaveAs(Environment.CurrentDirectory + "\\" + _informationModel.Name + ".xls", XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlWorkBook.Close(true, misValue, misValue);
                    xlApp.Quit();

                    ReleaseObject(xlWorkSheet);
                    ReleaseObject(xlWorkBook);
                    ReleaseObject(xlApp);

                    SystemSounds.Exclamation.Play();
                }
                catch
                {
                    throw new Exception("Закрийте поточний Excel файл!");
                }
            }
            else
            {
                throw new Exception("Excel не встановлений, або встановлений неправильно!");
            }
        }

        private List<double> VolumesPerMillimetersArray()
        {
            int iterator = 0;
            double sum = 0;
            List<double> minVolumes = new List<double>();
            for (int i = 0; i < _calculationResult.VolumesBetweenHulls.Count; i++)
            {
                iterator++;
                sum += _calculationResult.VolumesBetweenHulls[i];

                if (iterator == 10)
                {
                    if (minVolumes.Count == 0)
                    {
                        minVolumes.Add(sum);
                    }
                    else
                    {
                        minVolumes.Add(minVolumes.Last() + sum);
                    }
                    iterator = 0;
                    sum = 0;
                }
                if (i == _calculationResult.VolumesBetweenHulls.Count - 1)
                {
                    if (minVolumes.Count > 1)
                    {
                        minVolumes.Add(minVolumes.Last() + sum);
                    }
                    else
                    {
                        minVolumes.Add(sum);
                    }
                }
            }
            return minVolumes;
        }

        private double Percentage(double vol)
        {
            return 100 * (vol / _calculationResult.Volume);
        }

        private double GetRadius(List<Point> centralPereriz, Point centroidOfCentralPereriz)
        {
            double maxDist = 0, minDist = 0, tempDist = 0;

            foreach (var p in centralPereriz)
            {
                tempDist = DistanceBetweenPoints(centroidOfCentralPereriz, p);
                if (maxDist == 0 && minDist == 0)
                {
                    maxDist = minDist = tempDist;
                }
                else if (tempDist > maxDist)
                {
                    maxDist = tempDist;
                }
                else if (tempDist < minDist)
                {
                    minDist = tempDist;
                }
            }

            return (maxDist + minDist) / 2;
        }

        private double DistanceBetweenPoints(Point a, Point b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        private void MakeBorderCells(ref Worksheet worksheet, string from, string to)
        {
            worksheet.get_Range(from, to).Cells.Borders.LineStyle = XlLineStyle.xlContinuous;
        }

        private void WrapText(ref Worksheet worksheet, string from, string to)
        {
            worksheet.get_Range(from, to).WrapText = true;
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                throw new Exception("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
