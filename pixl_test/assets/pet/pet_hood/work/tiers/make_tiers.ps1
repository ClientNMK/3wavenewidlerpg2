param($r,$t)
Add-Type -Path "$r\pixl_test\assets\pet\pet_hood\work\tools\CloakTone.cs" -ReferencedAssemblies System.Drawing
$tiers=[ordered]@{ "t2_mint"=@(165,0.32,1.0,0.0); "t3_coral"=@(8,0.55,1.0,0.0); "t4_lavender"=@(262,0.30,1.0,0.0); "t5_gold"=@(42,0.62,1.0,0.0); "t6_night"=@(228,0.38,0.72,-0.02) }
foreach($n in "hood_001","hood_002"){ foreach($k in $tiers.Keys){ $a=$tiers[$k]; [CloakTone]::Apply("$r\ArtSource\pets\pet_hood\$n\pose\${n}_se.png","$t\${n}_$k.png",$a[0],$a[1],$a[2],$a[3]) } }
