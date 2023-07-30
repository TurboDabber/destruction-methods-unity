// Voronoi calculation example code
//
// Author   : Chris H. Rycroft (LBL / UC Berkeley)
// Email    : chr@alum.mit.edu
// Date     : August 30th 2011

#include "voro++.hh"
#include <fstream>
#include <vector>
#include <cstdlib>
using namespace voro;
 using namespace std;
const int n_x=10,n_y=10,n_z=10;
double x_min=-1,x_max=1;
double y_min=-1,y_max=1;
double z_min=-1,z_max=1;
// Set up constants for the container geometry
double cvol=(x_max-x_min)*(y_max-y_min)*(x_max-x_min);

// Set up the number of blocks that the container is divided into

// Set the number of particles that are going to be randomly introduced

// This function returns a random double between 0 and 1
double rnd() {return double(rand())/RAND_MAX;}
void draw_polygon(FILE *fp,vector<int> &f_vert,vector<double> &v,int j, double x, double y, double z);

int main(int argc, char* argv[]) {
	//args definition

    int particles = 4;

    // Check if command-line arguments are provided for the constants
    if (argc >= 5) {
        double bound_x = std::atoi(argv[1]);
		x_min=-bound_x/2.0;
		x_max=bound_x/2.0;
        double bound_y = std::atoi(argv[2]);
		y_min=-bound_y/2.0;
		y_max= bound_y/2.0;
        double bound_z = std::atoi(argv[3]);
		z_min=-bound_z/2.0;
		z_max= bound_z/2.0;
        particles = std::atoi(argv[4]);
		cvol=(x_max-x_min)*(y_max-y_min)*(x_max-x_min);
    }
	
	int i;
	double x,y,z;
	
	// Create a container with the geometry given above, and make it
	// non-periodic in each of the three coordinates. Allocate space for
	// eight particles within each computational block
	container con(x_min,x_max,y_min,y_max,z_min,z_max,n_x,n_y,n_z,
			false,false,false,8);

	// Randomly add particles into the container
	for(i=0;i<particles;i++) {
		x=x_min+rnd()*(x_max-x_min);
		y=y_min+rnd()*(y_max-y_min);
		z=z_min+rnd()*(z_max-z_min);
		con.put(i,x,y,z);
	}

	// Sum up the volumes, and check that this matches the container volume
	double vvol=con.sum_cell_volumes();
	printf("Container volume : %g\n"
	       "Voronoi volume   : %g\n"
	       "Difference       : %g\n",cvol,vvol,vvol-cvol);

	// Output the particle positions in gnuplot format
	//con.draw_particles("random_points_p.gnu");

	// Output the Voronoi cells in gnuplot format
	//con.draw_cells_gnuplot("random_points_v.gnu");
	
    // Loop over all particles in the container and compute each Voronoi
    // cell
	FILE *fp4=safe_fopen("Assets\\result.txt","w");
		 voronoicell_neighbor c;
		 vector<int> neigh,f_vert;
		 vector<double> v;
         c_loop_all cl(con);
		 double x1,y1,z1;
		 int id;
		 int i1, j1;
         if(cl.start()) do if(con.compute_cell(c,cl)) {
                 cl.pos(x1,y1,z1);id=cl.pid();
 
                 // Gather information about the computed Voronoi cell
                 c.neighbors(neigh);
                 c.face_vertices(f_vert);
                 c.vertices(x1,y1,z1,v);
				 fprintf(fp4, "-c\n");
				 fprintf(fp4, "%f;%f;%f\n>>>\n", x1, y1, z1);
				 
                 // Loop over all faces of the Voronoi cell
                 for(i1=0,j1=0;i1<neigh.size();i1++) {
						 draw_polygon(fp4,f_vert,v,j1, x1, y1, z1);
                         j1+=f_vert[j1]+1;
                 }
         } while (cl.inc());
 
         // Close the output files
         fclose(fp4);
	
    return 0;
}

void draw_polygon(FILE *fp,vector<int> &f_vert,vector<double> &v,int j, double x, double y, double z) {
       static char s[6][128];
    int k, l, n = f_vert[j];

    // Create POV-Ray vector strings for each of the vertices
    for (k = 0; k < n; k++) {
        l = 3 * f_vert[j + k + 1];
        sprintf(s[k], "%g,%g,%g", v[l], v[l + 1], v[l + 2]);
    }

    // Calculate the signed volume of the tetrahedron formed by the triangle and the point (x, y, z)
    double signedVolume = 0.0;
    for (k = 2; k < n; k++) {
        int idx0 = 3 * f_vert[j + 1];
        int idx1 = 3 * f_vert[j + k];
        int idx2 = 3 * f_vert[j + k + 1];

        double v0x = v[idx0] - x;
        double v0y = v[idx0 + 1] - y;
        double v0z = v[idx0 + 2] - z;

        double v1x = v[idx1] - x;
        double v1y = v[idx1 + 1] - y;
        double v1z = v[idx1 + 2] - z;

        double v2x = v[idx2] - x;
        double v2y = v[idx2 + 1] - y;
        double v2z = v[idx2 + 2] - z;

        signedVolume += (v0x * (v1y * v2z - v1z * v2y) +
                         v0y * (v1z * v2x - v1x * v2z) +
                         v0z * (v1x * v2y - v1y * v2x)) / 6.0;
    }

    // Determine the winding based on the sign of the volume
    bool reverseWinding = signedVolume < 0;
    // Output the vertices with corrected winding
    if (reverseWinding) {
        for (k = 2; k < n; k++) {
            fprintf(fp, "t:%s;%s;%s\n", s[0], s[k], s[k - 1]);
        }
    }
    else {
        for (k = 2; k < n; k++) {
            fprintf(fp, "t:%s;%s;%s\n", s[0], s[k - 1], s[k]);
        }
    }
 }
 