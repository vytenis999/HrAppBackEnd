namespace MouseTagProject.DTOs
{
    public class TaxInputDto
    {
        public string year { get; set; }
        public string salary { get; set; }
        public string salary_type { get; set; } // ant popieriaus 1, i rankas 2
        public string npd { get; set; } // paskaiciuoja sistema 1, nurodysiu pats 2
        public string npd_pats { get; set; } // 0
        public string pensija_papil { get; set; } // 0
        public string pensija_kiekis { get; set; } // 0
    }
}
