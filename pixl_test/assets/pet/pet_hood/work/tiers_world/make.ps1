param($r,$t)
Add-Type -Path "$r\pixl_test\assets\pet\pet_hood\work\tools\CloakTone.cs" -ReferencedAssemblies System.Drawing
$tiers=[ordered]@{ "w2_forest"=@(88,0.30,0.97,0.0); "w3_toy"=@(2,0.52,0.98,0.0); "w4_book"=@(40,0.50,0.98,0.0); "w5_deep"=@(232,0.30,0.70,0.0) }
foreach($n in "hood_001","hood_002"){ foreach($k in $tiers.Keys){ $a=$tiers[$k]; [CloakTone]::Apply("$r\ArtSource\pets\pet_hood\$n\pose\${n}_se.png","$t\${n}_$k.png",$a[0],$a[1],$a[2],$a[3]) } }
