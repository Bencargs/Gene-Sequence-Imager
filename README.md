# Gene Sequence Imager
Build visual representations of genetic code
<p align="center">
  <img 
    src="https://camo.githubusercontent.com/fd53c822ce00b9f1390fa43731f3d352cbd61767/68747470733a2f2f696d6167652e7368757474657273746f636b2e636f6d2f7a2f73746f636b2d766563746f722d69636f6e2d6f662d7468652d7374727563747572652d6f662d7468652d646e612d6d6f6c6563756c652d73706972616c2d64656f78797269626f6e75636c6569632d616369642d646e612d776974682d666f726d756c612d616e642d313034313839383337382e6a7067"
    width="200" 
    height=200"
  >
</p>

Using data from the [National Centre for Biotechnology Information](https://www.ncbi.nlm.nih.gov/genbank/sars-cov-2-seqs/) 
the RNA sequence for SARS-CoV-2 can be downloaded. 
eg. [2020-03-2020.txt](https://github.com/Bencargs/Gene-Sequence-Imager/blob/master/17-04-2020.txt)

As the sequence of the virus is 29,841 nucleotides long, an image of 174x174 can be constructed. 
Assigning colours to each of the bases; g = red, a = blue, c = green, t = yellow, and n = white (where n represents any Nucleotide)

Producing an image represenging viral RNA -
<p align="center">
  <img src="https://github.com/Bencargs/Gene-Sequence-Imager/blob/master/AUS%2025-01-2020.png">
</p>

As the virus mutates over time, sections of the genome change at differant rates.
sections critical to the virus' function and survival should remain unchanged however.
These sections of low of low mutation can be highlighted by by overlaying sequence images into a heatmap.
<p align="center">
  <img src="https://github.com/Bencargs/Gene-Sequence-Imager/blob/master/Heatmap.png">
  <img src="https://github.com/Bencargs/Gene-Sequence-Imager/blob/master/viridisScale.png" height="173">
</p>
Darker areas indicate lower mutation frequency - [viridis scale](https://cran.r-project.org/web/packages/viridis/vignettes/intro-to-viridis.html)


Future work:
Create video showing virual mutation over time.
- The assumption that slowly mutation sections of the genome should be sections important for virus function, eg spike proteins. 
Sections of higher frequency of mutation should be less important for viral survival.
