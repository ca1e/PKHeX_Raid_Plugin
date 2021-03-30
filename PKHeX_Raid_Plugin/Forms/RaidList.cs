using PKHeX.Core;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX_Raid_Plugin
{
    public partial class RaidList : Form
    {
        private readonly RaidManager _raids;
        private readonly TextBox[] IVs;

        public RaidList(SaveBlockAccessor8SWSH blocks, GameVersion game, int badges, int tid, int sid)
        {
            InitializeComponent();
            IVs = new[] { TB_HP_IV1, TB_ATK_IV1, TB_DEF_IV1, TB_SPA_IV1, TB_SPD_IV1, TB_SPE_IV1 };
            _raids = new RaidManager(blocks, game, badges, (uint)tid, (uint)sid);
            CB_Den.SelectedIndex = 0;
            foreach (int item in Enumerable.Range(1, 100))
            {
                CB_mRandRoll.Items.Add(item.ToString());
            }
            CB_Den.SelectedIndex = 0;
            CenterToParent();
        }

        private void Changessrr(object sender, EventArgs e)
        {
            RaidParameters raidParameters = _raids[CB_Den.SelectedIndex];
            RaidParameters raidParameters2 = new RaidParameters(raidParameters.Index, raidParameters.Seed, CB_mStars.SelectedIndex, CB_mRandRoll.SelectedIndex + 1, raidParameters.Flags, raidParameters.Type, 0, raidParameters.X, raidParameters.Y);
            RaidPKM raidPKM = _raids.GenerateFromIndex(raidParameters2);
            GameStrings strings = GameInfo.GetStrings(GameInfo.CurrentLanguage);
            L_Ability.Text = "Ability: " + strings.Ability[raidPKM.Ability];
            L_Nature.Text = "Nature: " + strings.natures[raidPKM.Nature];
            PB_PK1.BackgroundImage = RaidUtil.GetRaidResultSprite(raidPKM, active: true);
            L_Stars.Text = RaidUtil.GetStarString(raidParameters2);
        }

        private void ChangeDenIndex(object sender, EventArgs e) => LoadDen(_raids[CB_Den.SelectedIndex]);

        private void ShowDenIVs(object sender, EventArgs e)
        {
            using var divs = new DenIVs(CB_Den.SelectedIndex, _raids);
            divs.ShowDialog();
        }

        private void LoadDen(RaidParameters raidParameters)
        {
            try
            {
                CB_mStars.SelectedIndex = raidParameters.Stars;
                CB_mRandRoll.SelectedIndex = raidParameters.RandRoll - 1;
            }
            catch (Exception)
            {
                MessageBox.Show("Stars or RandRoll are invalid");
            }
            CHK_Active.Checked = raidParameters.IsActive;
            CHK_Rare.Checked = raidParameters.IsRare;
            CHK_Event.Checked = raidParameters.IsEvent;
            CHK_Wishing.Checked = raidParameters.IsWishingPiece;
            CHK_Watts.Checked = raidParameters.WattsHarvested;
            L_DenSeed.Text = $"{raidParameters.Seed:X16}";
            L_Stars.Text = RaidUtil.GetStarString(raidParameters);

            var pkm = _raids.GenerateFromIndex(raidParameters);
            var s = GameInfo.Strings;
            L_Ability.Text = $"Ability: {s.Ability[pkm.Ability]}";
            L_Nature.Text = $"Nature: {s.natures[pkm.Nature]}";
            L_ShinyInFrames.Text = $"Next Shiny Frame: {RandUtil.GetNextShinyFrame(raidParameters.Seed)}";
            L_Shiny.Visible = pkm.ShinyType != 0;
            L_Shiny.Text = pkm.ShinyType == 1 ? "Shiny: Star" : pkm.ShinyType == 2? (pkm.ForcedShinyType == 2 ? "Shiny: Forced Square" : "Shiny: Square!!!") : "Shiny locked";

            for (int i = 0; i < 6; i++) { 
                IVs[i].Text = $"{pkm.IVs[i]:00}";
            }

            PB_PK1.BackgroundImage = RaidUtil.GetRaidResultSprite(pkm, CHK_Active.Checked);
            L_Location.Text = raidParameters.Location;

            if (raidParameters.X > 0 && raidParameters.Y > 0)
                DenMap.BackgroundImage = RaidUtil.GetNestMap(raidParameters);
        }

        private void PB_PK1_DoubleClick(object sender, EventArgs e)
        {
            RaidParameters raidParameters = _raids[CB_Den.SelectedIndex];
            RaidParameters raidParameters2 = new RaidParameters(raidParameters.Index, raidParameters.Seed, CB_mStars.SelectedIndex, CB_mRandRoll.SelectedIndex + 1, raidParameters.Flags, raidParameters.Type, 0, raidParameters.X, raidParameters.Y);
            RaidPKM raidPkm = _raids.GenerateFromIndex(raidParameters2);
            PB_PK1.BackgroundImage = RaidUtil.GetRaidResultSprite(raidPkm, CHK_Active.Checked);
        }
    }
}
