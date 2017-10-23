using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*The main idea to implement this algorithm comes form Jaap Scherphuis */

namespace MagicCube_Refined
{
    class MagicCubeRecover
    {
        private string Stranslated_input_;
	    private string faces_;
        private string tmpt_out_ = "";
        private string update_tmpt_ = "";
	    private char[] order_;
	    private char[] bithash_;
	    private char[] perm_;
	    private char[] pos_ = new char[20];
        private char[] ori_ = new char[20];
        private char[] val_ = new char[20];
	    private char[][] tables = new char[8][];
        private char[][] transfer_cube_ = new char[6][];
        private char[][] cube_1_ = new char[6][];
        int[] move_ = new int[20];
        int[] moveamount_ = new int[20];
	    private int phase_;
	    private int[] tablesize_ = new int[8];
        bool is_inilization_ = false;
        int CHAROFFSET;

        public MagicCubeRecover()
        {

        }
        public void LoadCube(char[][] cube)			// load cube from an color array:B-F,R-R,G-B,Y-L,Y-U,W-D, where F,R,B,L,U,or D represent the oritation
        {
            initialize();
            // Transfer cube to a given order
           for(int i=0;i<6;i++)
            {
                cube_1_[i] = new char[9];
                for (int j = 0; j < 9; j++)
                    cube_1_[i][j] = cube[i][j];
            }
            // The front face
            transfer_cube_[0] = new char[9];
            for (int i = 0; i < 9;i++ )
                transfer_cube_[0][i] = cube[4][i];

            // The right face
            transfer_cube_[1] = new char[9];
            transfer_cube_[1][0] = cube[2][6];
            transfer_cube_[1][1] = cube[2][3];
            transfer_cube_[1][2] = cube[2][0];
            transfer_cube_[1][3] = cube[2][7];
            transfer_cube_[1][4] = cube[2][4];
            transfer_cube_[1][5] = cube[2][1];
            transfer_cube_[1][6] = cube[2][8];
            transfer_cube_[1][7] = cube[2][5];
            transfer_cube_[1][8] = cube[2][2];

            // The back face
            transfer_cube_[2] = new char[9];
            for (int i = 0; i < 9; i++)
                transfer_cube_[2][i] = cube[5][8 - i];

            // The left face
            transfer_cube_[3] = new char[9];
            transfer_cube_[3][0] = cube[0][2];
            transfer_cube_[3][1] = cube[0][5];
            transfer_cube_[3][2] = cube[0][8];
            transfer_cube_[3][3] = cube[0][1];
            transfer_cube_[3][4] = cube[0][4];
            transfer_cube_[3][5] = cube[0][7];
            transfer_cube_[3][6] = cube[0][0];
            transfer_cube_[3][7] = cube[0][3];
            transfer_cube_[3][8] = cube[0][6];

            // The up face
            transfer_cube_[4] = new char[9];
            for (int i = 0; i < 9; i++)
                transfer_cube_[4][i] = cube[3][i];

            // The down face
            transfer_cube_[5] = new char[9];
            for (int i = 0; i < 9; i++)
                transfer_cube_[5][i] = cube[1][8 - i];

            setedgepieces(transfer_cube_);
            setcornerpieces(transfer_cube_);
            is_inilization_ = true;
        }

        public void  RecoverCube(ref string operations) // Recover the cube
        {
            if (is_inilization_ != true)
                return;
            phase_ = 0;

            string[] split_ = Stranslated_input_.Split(' ');
            int f, i = 0, j = 0, k = 0, pc, mor;

            for (; k < 20; k++) val_[k] = Convert.ToChar(k < 12 ? 2 : 3);
            for (; j < 8; j++) filltable(j);

            for (; i < 20; i++)
            {
                f = pc = k = mor = 0;
                for (; f < val_[i]; f++)
                {
                    j = faces_.IndexOf(split_[i][f]);
                    if (j > k) { k = j; mor = f; }
                    pc += 1 << j;
                }
                for (f = 0; f < 20; f++)
                    if (pc == bithash_[f] - 64) break;

                pos_[order_[i] - CHAROFFSET] = Convert.ToChar(f);
                ori_[order_[i] - CHAROFFSET] = Convert.ToChar(mor % val_[i]);
            }
           for (; phase_ < 8; phase_ += 2)
            {
                for (j = 0; !searchphase(j, 0, 9); j++) ;
                for (i = 0; i < j; i++)
                {
                   // for (int l = 0; l < moveamount_[i]; l++)
                        //operations += "FBRLUD"[move_[i]];
                    update_tmpt_ += "FBRLUD"[move_[i]] + moveamount_[i].ToString();
                }
            }
            //update_tmpt_ = "F3F1F2D1D2U1U2R1R2R3";
            int size_ = update_tmpt_.Length;
            if (size_ < 4)
                for (i = 0; i < size_; i++)
                    tmpt_out_ += update_tmpt_[i];
            else
            {
                for (i = 0; i < size_ ;)
                {
                    j = 2;
                    for (; i + j + 1 < size_; j = j + 2)
                        if (update_tmpt_[i] != update_tmpt_[i + j])
                            break;
                    int count_ = (j - 2) / 2;
                    int num_ = 0;
                    for (k = 0; k <= count_; k++)
                    {
                        num_ = num_ + update_tmpt_[i + 2 * k + 1] - '0';
                    }
                    num_ = num_ % 4;
                    if (num_ != 0)
                        tmpt_out_ += update_tmpt_[i] + num_.ToString();
                    i = i + j;
                    /*if (i + 3 < size_)
                    {
                        if (update_tmpt_[i] == update_tmpt_[i + 2])
                        {
                            int num_ = update_tmpt_[i + 1] - '0' + update_tmpt_[i + 3] - '0';
                            num_ = num_ % 4;
                            if (num_ != 0)
                                tmpt_out_ += update_tmpt_[i] + num_.ToString();
                            i = i + 4;
                        }
                        else
                        {
                            tmpt_out_ += update_tmpt_[i];
                            tmpt_out_ += update_tmpt_[i + 1];
                            i = i + 2;
                        }
                    }*/
                    /* else
                     {
                         tmpt_out_ += update_tmpt_[i];
                         tmpt_out_ += update_tmpt_[i + 1];
                         i = i + 2;
                     }*/

                }
            }
            size_ = tmpt_out_.Length;
            int flag_ = calculate_count();
            if (flag_ == 0)
            {
                operations += flag_.ToString();
                operations += flag_.ToString();
                for (i = 0; i < size_; i++)
                    operations += tmpt_out_[i];
            }
            else if(flag_==1)
            {
                operations += flag_.ToString();
                operations += flag_.ToString();
                for (i = 0; i < size_; i = i + 2)
                {
                    switch (tmpt_out_[i])
                    {
                        case 'U':
                            operations += 'F';
                            break;
                        case 'D':
                            operations += 'B';
                            break;
                        case 'F':
                            operations += 'D';
                            break;
                        case 'B':
                            operations += 'U';
                            break;
                        case 'L':
                            operations += 'L';
                            break;
                        case 'R':
                            operations += 'R';
                            break;

                    }
                    operations += tmpt_out_[i + 1];
                }
            }
            else
            {
                operations += flag_.ToString();
                operations += flag_.ToString();
                for (i = 0; i < size_; i = i + 2)
                {
                    switch (tmpt_out_[i])
                    {
                        case 'U':
                            operations += 'L';
                            break;
                        case 'D':
                            operations += 'R';
                            break;
                        case 'F':
                            operations += 'F';
                            break;
                        case 'B':
                            operations += 'B';
                            break;
                        case 'L':
                            operations += 'D';
                            break;
                        case 'R':
                            operations += 'U';
                            break;

                    }
                    operations += tmpt_out_[i + 1];
                }
            }
         }

        private int calculate_count()
        {
            if (tmpt_out_.Length == 0)
                return 0;
            int flag_=0;
            int UD_num_=0, LR_num_=0, FB_num_=0;
            int size_ = tmpt_out_.Length;
            for (int i = 0; i < size_;i=i+2 )
            {
                switch(tmpt_out_[i])
                {
                    case 'U':
                        {
                            if (tmpt_out_[i + 1] == '1' || tmpt_out_[i + 1] == '3')
                                UD_num_++;
                            else
                                UD_num_ += 2;
                        }
                    break;
                    case 'D':
                    {
                        if (tmpt_out_[i + 1] == '1' || tmpt_out_[i + 1] == '3')
                            UD_num_++;
                        else
                            UD_num_ += 2;
                    }
                    break;
                    case 'L':
                    {
                        if (tmpt_out_[i + 1] == '1' || tmpt_out_[i + 1] == '3')
                            LR_num_++;
                        else
                            LR_num_ += 2;
                    }
                    break;
                    case 'R':
                    {
                        if (tmpt_out_[i + 1] == '1' || tmpt_out_[i + 1] == '3')
                            LR_num_++;
                        else
                            LR_num_ += 2;
                    }
                    break;
                    case 'F':
                    {
                        if (tmpt_out_[i + 1] == '1' || tmpt_out_[i + 1] == '3')
                            FB_num_++;
                        else
                            FB_num_ += 2;
                    }
                    break;
                    case 'B':
                    {
                        if (tmpt_out_[i + 1] == '1' || tmpt_out_[i + 1] == '3')
                            FB_num_++;
                        else
                            FB_num_ += 2;
                    }
                    break;
                    default:
                    break;
                }
            }
            if (UD_num_ < FB_num_ && UD_num_ < LR_num_)
                flag_ = 0;
            else if (FB_num_ < LR_num_ && FB_num_ < UD_num_)
                flag_ = 1;
            else
                flag_ = 2;

            return flag_;
        }
        private void initialize()   // initialize the cube
        {
            faces_ = "RLFBUD";
            //Stranslated_input_ = "RU LF UB DR DL BL UL FU BD RF BR FD LDF LBD FUL RFD UFR RDB UBL RBU";
	        order_ = "AECGBFDHIJKLMSNTROQP".ToCharArray();
	        bithash_ = "TdXhQaRbEFIJUZfijeYV".ToCharArray();
	        perm_ = "AIBJTMROCLDKSNQPEKFIMSPRGJHLNTOQAGCEMTNSBFDHORPQ".ToCharArray();
	        tablesize_[0] = 1; tablesize_[1] = 4096; tablesize_[2] = 6561; tablesize_[3] = 4096;
	        tablesize_[4] = 256;tablesize_[5] = 1536; tablesize_[6] = 13824; tablesize_[7] = 576;
	        phase_ = 0;
            CHAROFFSET = 65;
        }

        private void setedgepieces(char[][] cube)		// set the edge-side pieces color in the order of UF UR UB UL DF DR DB DL FR FL BR BL 
        {
            char color_='0';
            // The edge-side pieces in the top layer
            // UF
            setcolor(cube[4][7], ref color_);
            Stranslated_input_+=color_;
            setcolor(cube[0][1], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //UR
            setcolor(cube[4][5], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[1][1], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //UB
            setcolor(cube[4][1], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[2][1], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //UL
            setcolor(cube[4][3], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[3][1], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';


            // The edge-side pieces in the bottom layer
            // DF
            setcolor(cube[5][1], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[0][7], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //DR
            setcolor(cube[5][5], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[1][7], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //DB
            setcolor(cube[5][7], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[2][7], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //DL
            setcolor(cube[5][3], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[3][7], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';


            // The edge-side pieces in the middel layer
            //FR
            setcolor(cube[0][5], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[1][3], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //FL
            setcolor(cube[0][3], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[3][5], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //BR
            setcolor(cube[2][3], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[1][5], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //BL
            setcolor(cube[2][5], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[3][3], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';
	
        }

        private void setcornerpieces(char[][] cube)        // set the corner pieces color in the order of UFR URB UBL ULF DRF DFL DLB DBR
        {
            // The corner pieces in the top layer
            char color_=' ';
            //UFR
            setcolor(cube[4][8], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[0][2], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[1][0], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //URB
            setcolor(cube[4][2], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[1][2], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[2][0], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //UBL
            setcolor(cube[4][0], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[2][2], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[3][0], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //ULF
            setcolor(cube[4][6], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[3][2], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[0][0], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';


            // The corner pieces in the bottom layer
            //DRF 
            setcolor(cube[5][2], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[1][6], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[0][8], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //DFL 
            setcolor(cube[5][0], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[0][6], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[3][8], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //DLB
            setcolor(cube[5][6], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[3][6], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[2][8], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';

            //DBR
            setcolor(cube[5][8], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[2][6], ref color_);
            Stranslated_input_ += color_;
            setcolor(cube[1][8], ref color_);
            Stranslated_input_ += color_;
            Stranslated_input_ += ' ';
	
        }

        private void setcolor(char i,ref char color)		// set the piece color B-F,R-R,G-B,Y-L,Y-U,W-D, where F,R,B,L,U,or D represent the oritation
        {
            switch (i)
            {
                case 'B':
                    color = 'F';
                    break;
                case 'R':
                    color = 'R';
                    break;
                case 'G':
                    color = 'B';
                    break;
                case 'O':
                    color = 'L';
                    break;
                case 'Y':
                    color = 'U';
                    break;
                case 'W':
                    color = 'D';
                    break;
                default:
                    break;
            }
        }

       
        private int Char2Num(char c)                    // transfer char to int
        {
            return (int)c - CHAROFFSET;
        }

        private void SWAP(ref char a, ref char b)       // exchange a and b
        {
            char temp_ = a;
            a = b;
            b = temp_;
        }
        private void cycle(char[] p, char[] a, int offset_)		// swap the 4 pieces in array b in cycle, the indices are given by a, and the parametor offset store the position of the started element in a
        {
            SWAP(ref p[Char2Num(a[0 + offset_])], ref p[Char2Num(a[1 + offset_])]);
            SWAP(ref p[Char2Num(a[0 + offset_])], ref p[Char2Num(a[2 + offset_])]);
            SWAP(ref p[Char2Num(a[0 + offset_])], ref p[Char2Num(a[3 + offset_])]);
        }
        private void twist(int i, int times)            	// twist the i-th piece times_+1 times
        {
            i -= CHAROFFSET;
            ori_[i] = Convert.ToChar(((int)ori_[i] + times + 1) % val_[i]);
        }
        private void reset()							// set cube to solved position  
        {
            for (int i = 0; i < 20; pos_[i] = Convert.ToChar(i), ori_[i++] = '\0') ;
        }
        private int permtonum(char[] p, int offset_)					// convert permutation of 4 chars to a number in range 0..23,and offset_ is used to store the started position
        {
            int n = 0;
            for (int a = 0; a < 4; a++)
            {
                n *= 4 - a;
                for (int b = a; ++b < 4; )
                    if (p[b+offset_] < p[a+offset_]) n++;
            }
            return n;
        }
        private void numtoperm(char[] p, int n, int o)	// convert number in range 0..23 to permutation of 4 chars.
        {
            p[3 + o] = Convert.ToChar(o);
            for (int a = 3; a-- > 0; )
            {
                p[a + o] = Convert.ToChar(n % (4 - a) + o);
                n /= 4 - a;
                for (int b = a; ++b < 4; )
                    if (p[b + o] >= p[a + o]) p[b + o]++;
            }
        }
        private int getposition(int t)					// get index of cube position from table t
        {
           int i = -1, n = 0;
	        switch (t){
		    // case 0 does nothing so returns 0
	        case 1://edgeflip
		    // 12 bits, set bit if edge is flipped
		    for (; ++i<12;) n += ((int)ori_[i]) << i;
		        break;
	        case 2://cornertwist
		    // get base 3 number of 8 digits - each digit is corner twist
		    for (i = 20; --i>11;) n = n * 3 + (int)ori_[i];
		        break;
	        case 3://middle edge choice
		    // 12 bits, set bit if edge belongs in Um middle slice
		    for (; ++i<12;) n += ((((int)pos_[i]) & 8)>0) ? (1 << i) : 0;
		        break;
	        case 4://ud slice choice
		    // 8 bits, set bit if UD edge belongs in Fm middle slice
		    for (; ++i<8;) n += ((((int)pos_[i]) & 4)>0) ? (1 << i) : 0;
		       break;
	        case 5://tetrad choice, twist and parity
		    int[] corn = new int[8];
            int j, k, l;
            int[] corn2 = new int[4];
		    // 8 bits, set bit if corner belongs in second tetrad.
		    // also separate pieces for twist/parity determination
		    k = j = 0;
		    for (; ++i<8;)
		    if (((l = pos_[i + 12] - 12) & 4) > 0)
            {
			    corn[l] = k++;
			    n += 1 << i;
		    }
		    else corn[j++] = l;
		    //Find permutation of second tetrad after solving first
		    for (i = 0; i<4; i++) corn2[i] = corn[4 + corn[i]];
		    //Solve one piece of second tetrad
		    for (; --i>0;) corn2[i] ^= corn2[0];

		    // encode parity/tetrad twist
		    n = n * 6 + corn2[1] * 2 - 2;
		    if (corn2[3]<corn2[2])n++;
		        break;
	        case 6://two edge and one corner orbit, permutation
		        n = permtonum(pos_,0) * 576 + permtonum(pos_ , 4) * 24 + permtonum(pos_ , 12);
		    break;
	        case 7://one edge and one corner orbit, permutation
		        n = permtonum(pos_ , 8) * 24 + permtonum(pos_ , 16);
		    break;
	    }
	        return n;
        }
        private void setposition(int t, int n)			// sets cube to any position which has index n in table t
        {
          int i = 0, j = 12, k = 0;
          char[] corn = "QRSTQRTSQSRTQTRSQSTRQTSR".ToCharArray();
            reset();
            switch (t)
            {
                // case 0 does nothing so leaves cube solved
                case 1://edgeflip
                    for (; i < 12; i++, n >>= 1) ori_[i] = Convert.ToChar(n & 1);
                    break;
                case 2://cornertwist
                    for (i = 12; i < 20; i++, n /= 3) ori_[i] = Convert.ToChar(n % 3);
                    break;
                case 3://middle edge choice
                    for (; i < 12; i++, n >>= 1) pos_[i] = Convert.ToChar(8 * n & 8);
                    break;
                case 4://ud slice choice
                    for (; i < 8; i++, n >>= 1) pos_[i] = Convert.ToChar(4 * n & 4);
                    break;
                case 5://tetrad choice,parity,twist
                    int offset_ = n % 6 * 4;
                    n /= 6;
                    for (; i < 8; i++, n >>= 1)
                        pos_[i + 12] =Convert.ToChar(((n & 1)>0) ? corn[offset_ + k++] - CHAROFFSET : j++);
                    break;
                case 6://slice permutations
                    numtoperm(pos_, n % 24, 12); n /= 24;
                    numtoperm(pos_, n % 24, 4); n /= 24;
                    numtoperm(pos_, n, 0);
                    break;
                case 7://corner permutations
                    numtoperm(pos_, n / 24, 8);
                    numtoperm(pos_, n % 24, 16);
                    break;
            }
        }
        private void domove(int m)						// do a clockwise quarter turn cube move
        {
            //char* p = perm + 8 * m;
            int offset = 8 * m;
            int i = 8;
            //cycle the edges
            cycle(pos_, perm_, offset);
            cycle(ori_, perm_, offset);
            //cycle the corners
            cycle(pos_, perm_, offset + 4);
            cycle(ori_, perm_, offset + 4);
            //twist corners if RLFB
            if (m < 4)
                for (; --i > 3; ) twist(perm_[i + offset], i & 1);
            //flip edges if FB
            if (m < 2)
                for (i = 4; i-- > 0; ) twist(perm_[i + offset], 0);
        }
        private void filltable(int ti)					// calculate a pruning table
        {
           int n = 1, l = 1, tl = tablesize_[ti];
            char[] tb = new char[tl];
            tables[ti] = tb;
            for (int i = 0; i < tb.Length; i++) tb[i] = '\0';

            reset();
            tb[getposition(ti)] = Convert.ToChar(1);

            // while there are positions of depth l
            while (n > 0)
            {
                n = 0;
                // find each position of depth l
                for (int i = 0; i < tl; i++)
                {
                    if (tb[i] == l)
                    {
                        //construct that cube position
                        setposition(ti, i);
                        // try each face any amount
                        for (int f = 0; f < 6; f++)
                        {
                            for (int q = 1; q < 4; q++)
                            {
                                domove(f);
                                // get resulting position
                                int r = getposition(ti);
                                // if move as allowed in that phase, and position is a new one
                                if ((q == 2 || f >= (ti & 6)) && tb[r] == '\0')
                                {
                                    // mark that position as depth l+1
                                    tb[r] = Convert.ToChar(l + 1);
                                    n++;
                                }
                            }
                            domove(f);
                        }
                    }
                }
                l++;
            }
        }
        private bool searchphase(int movesleft, int movesdone, int lastmove)
        // Pruned tree search. recursive.
        {
            // prune - position must still be solvable in the remaining moves available
            if (tables[phase_][getposition(phase_)] - 1 > movesleft ||
                tables[phase_ + 1][getposition(phase_ + 1)] - 1 > movesleft) return false;

            // If no moves left to do, we have solved this phase
            if (movesleft==0) return true;

            // not solved. try each face move
            for (int i = 6; i-->0;)
            {
                // do not repeat same face, nor do opposite after DLB.
                if ((i - lastmove != 0) && ((i - lastmove + 1) != 0 || ((i | 1) != 0)))
                {
                    move_[movesdone] = i;
                    // try 1,2,3 quarter turns of that face
                    for (int j = 0; ++j < 4; )
                    {
                        //do move and remember it
                        domove(i);
                        moveamount_[movesdone] = j;
                        //Check if phase only allows half moves of this face
                        if ((j == 2 || i >= phase_) &&
                            //search on
                            searchphase(movesleft - 1, movesdone + 1, i)) return true;
                    }
                    // put face back to original position.
                    domove(i);
                }
            }
            // no solution found
            return false;

        }
       

    }
}
